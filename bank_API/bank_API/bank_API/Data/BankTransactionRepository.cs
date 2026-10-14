using bank_API.Model;
using Microsoft.EntityFrameworkCore;
namespace bank_API.Data;

public class BankTransactionRepository(BankDbContext context)
{
    public Task<BankTransaction?> FindAsync(Guid id, string serviceId) =>
        context.Transactions.AsNoTracking().SingleOrDefaultAsync(x => x.AttemptId == id && x.ServiceClientId == serviceId);

    public async Task<BankTransaction> SaveOutcomeAsync(BankTransaction transaction)
    {
        context.Transactions.Add(transaction);
        try { await context.SaveChangesAsync(); return transaction; }
        catch (DbUpdateException)
        {
            context.ChangeTracker.Clear();
            var stored = await context.Transactions.AsNoTracking().SingleOrDefaultAsync(x => x.AttemptId == transaction.AttemptId);
            if (stored == null) throw;
            if (stored.ServiceClientId != transaction.ServiceClientId || stored.Amount != transaction.Amount ||
                stored.Currency != transaction.Currency || stored.MockPaymentToken != transaction.MockPaymentToken)
                throw new InvalidOperationException("This reference belongs to a different request.");
            return stored;
        }
    }
}
