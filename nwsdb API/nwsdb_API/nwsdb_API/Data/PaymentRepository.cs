using System.Data;
using Microsoft.EntityFrameworkCore;
using nwsdb_API.DTO;
using nwsdb_API.Model;
namespace nwsdb_API.Data;

public class PaymentRepository(IDbContextFactory<AppDbContext> factory)
{
    private static IQueryable<PaymentAttempt> Details(AppDbContext db) => db.Attempts
        .Include(x => x.Bill).ThenInclude(x => x.Account).Include(x => x.Payment).ThenInclude(x => x!.Bill).ThenInclude(x => x.Account);
    private static IQueryable<PaymentAttempt> Authorized(IQueryable<PaymentAttempt> q, CallerContext caller) => caller.Role switch
    {
        UserRole.Staff => q,
        UserRole.PartnerCashier => q.Where(x => x.PartnerId == caller.PartnerId),
        _ => q.Where(x => false)
    };
    private static ReservationResult Replay(PaymentAttempt existing, CreatePaymentDto request) =>
        existing.BillId == request.BillId && existing.Amount == request.Amount && existing.MockPaymentToken == request.MockPaymentToken
        ? new() { Kind = ReservationKind.Replay, Attempt = existing }
        : new() { Kind = ReservationKind.Conflict, ConflictCode = "Reference already used with different payment details." };

    public async Task<ReservationResult> ReserveAsync(CreatePaymentDto request, int partnerId)
    {
        await using var db = await factory.CreateDbContextAsync();
        try
        {
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var existing = await Details(db).SingleOrDefaultAsync(x => x.PartnerId == partnerId && x.ClientReference == request.ClientReference);
            if (existing != null) return Replay(existing, request);
            if (!await db.Partners.AnyAsync(x => x.PartnerId == partnerId && x.IsActive)) throw new UnauthorizedAccessException("Partner is inactive.");
            var bill = await db.Bills.Include(x => x.Account).SingleOrDefaultAsync(x => x.BillId == request.BillId);
            if (bill == null || !bill.Account.IsActive) throw new KeyNotFoundException("Active bill not found.");
            if (request.Amount != bill.Amount || await db.Payments.AnyAsync(x => x.BillId == bill.BillId) ||
                await db.Attempts.AnyAsync(x => x.BillId == bill.BillId && x.Status == AttemptStatus.Pending))
                return new() { Kind = ReservationKind.Conflict, NoReservation = true, ConflictCode = "Bill must be unpaid, not processing, and paid in full." };
            var attempt = new PaymentAttempt { AttemptId = Guid.NewGuid(), PartnerId = partnerId, BillId = bill.BillId,
                Bill = bill, ClientReference = request.ClientReference, Amount = request.Amount, Currency = "LKR",
                MockPaymentToken = request.MockPaymentToken, Status = AttemptStatus.Pending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            db.Attempts.Add(attempt);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            return new() { Kind = ReservationKind.Reserved, Attempt = attempt };
        }
        catch (DbUpdateException)
        {
            // A concurrent request may have saved this reference first.
            await using var fresh = await factory.CreateDbContextAsync();
            var winner = await Details(fresh).AsNoTracking().SingleOrDefaultAsync(x => x.PartnerId == partnerId && x.ClientReference == request.ClientReference);
            if (winner != null) return Replay(winner, request);
            throw;
        }
    }
    public async Task<PaymentAttempt> FindAuthorizedAsync(Guid id, CallerContext caller)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await Authorized(Details(db).AsNoTracking(), caller).SingleOrDefaultAsync(x => x.AttemptId == id)
            ?? throw new KeyNotFoundException("Payment not found.");
    }
    public async Task<List<PaymentAttempt>> GetHistoryAsync(CallerContext caller)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await Authorized(Details(db).AsNoTracking(), caller).OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync();
    }
    public async Task<Payment> CompleteAsync(Guid id, BankPaymentResultDto result)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var attempt = await Details(db).SingleAsync(x => x.AttemptId == id);
        if (attempt.Status == AttemptStatus.Completed)
        {
            if (attempt.BankReference != result.BankReference) throw new InvalidOperationException("Stored payment result conflicts with bank response.");
            return attempt.Payment!;
        }
        if (attempt.Status != AttemptStatus.Pending) throw new InvalidOperationException("A declined payment cannot be completed.");
        var payment = new Payment { AttemptId = id, BillId = attempt.BillId, Bill = attempt.Bill,
            Amount = attempt.Amount, BankReference = result.BankReference, ReceiptNumber = "R-" + id.ToString("N"), PaidAt = result.ProcessedAt };
        db.Payments.Add(payment);
        attempt.Status = AttemptStatus.Completed; attempt.BankReference = result.BankReference; attempt.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return payment;
    }
    public async Task<PaymentAttempt> DeclineAsync(Guid id, BankPaymentResultDto result)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var attempt = await Details(db).SingleAsync(x => x.AttemptId == id);
        if (attempt.Status == AttemptStatus.Completed || (attempt.Status == AttemptStatus.Declined && attempt.BankReference != result.BankReference))
            throw new InvalidOperationException("Stored payment result conflicts with bank response.");
        attempt.Status = AttemptStatus.Declined; attempt.BankReference = result.BankReference; attempt.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(); await tx.CommitAsync(); return attempt;
    }
    public async Task<Payment> FindReceiptAuthorizedAsync(string number, CallerContext caller)
    {
        await using var db = await factory.CreateDbContextAsync();
        var q = db.Payments.AsNoTracking().Include(x => x.Bill).ThenInclude(x => x.Account).Where(x => x.ReceiptNumber == number);
        if (caller.Role == UserRole.Customer) q = q.Where(x => x.Bill.Account.CustomerId == caller.CustomerId);
        else if (caller.Role == UserRole.PartnerCashier) q = q.Where(x => db.Attempts.Any(a => a.AttemptId == x.AttemptId && a.PartnerId == caller.PartnerId));
        else if (caller.Role != UserRole.Staff) throw new UnauthorizedAccessException("Access denied.");
        return await q.SingleOrDefaultAsync() ?? throw new KeyNotFoundException("Receipt not found.");
    }
    public async Task<List<Payment>> GetCustomerPaymentsAsync(int customerId)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Payments.AsNoTracking().Include(x => x.Bill).ThenInclude(x => x.Account)
            .Where(x => x.Bill.Account.CustomerId == customerId).OrderByDescending(x => x.PaidAt).Take(100).ToListAsync();
    }
}
