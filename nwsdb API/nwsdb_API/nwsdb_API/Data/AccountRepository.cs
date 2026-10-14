using System.Data;
using Microsoft.EntityFrameworkCore;
using nwsdb_API.Model;
namespace nwsdb_API.Data;
public class AccountRepository(AppDbContext context)
{
    private IQueryable<WaterAccount> Details() => context.Accounts.AsNoTracking().AsSplitQuery()
        .Include(x => x.Customer).Include(x => x.Readings).Include(x => x.Bills).ThenInclude(x => x.Payment)
        .Include(x => x.Bills).ThenInclude(x => x.Attempts);
    public Task<List<WaterAccount>> GetOwnedAsync(int customerId) => Details().Where(x => x.CustomerId == customerId).ToListAsync();
    public Task<WaterAccount?> FindAsync(int id) => Details().SingleOrDefaultAsync(x => x.AccountId == id);
    public Task<WaterAccount?> FindByNumberAsync(string number) => Details().SingleOrDefaultAsync(x => x.AccountNumber == number);
    public Task<List<WaterAccount>> SearchAsync(string search, int page) => context.Accounts.AsNoTracking()
        .Where(x => x.AccountNumber.Contains(search) || x.ServiceAddress.Contains(search)).OrderBy(x => x.AccountId).Skip((page - 1) * 50).Take(50).ToListAsync();
    public Task<List<Customer>> SearchCustomersAsync(string search, int page) => context.Customers.AsNoTracking()
        .Where(x => x.FullName.Contains(search)).OrderBy(x => x.CustomerId).Skip((page - 1) * 50).Take(50).ToListAsync();
    public async Task<Customer> AddCustomerAsync(Customer customer)
    { context.Customers.Add(customer); await context.SaveChangesAsync(); return customer; }
    public async Task<WaterAccount> AddAsync(WaterAccount account)
    {
        if (!await context.Customers.AnyAsync(x => x.CustomerId == account.CustomerId)) throw new ArgumentException("Customer not found.");
        if (await context.Accounts.AnyAsync(x => x.AccountNumber == account.AccountNumber)) throw new InvalidOperationException("Account number already exists.");
        context.Accounts.Add(account); await context.SaveChangesAsync(); return account;
    }
    public async Task<MeterReading> AddReadingAsync(MeterReading reading)
    {
        await using var tx = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        if (!await context.Accounts.AnyAsync(x => x.AccountId == reading.AccountId && x.IsActive)) throw new KeyNotFoundException("Active account not found.");
        var last = await context.Readings.Where(x => x.AccountId == reading.AccountId).OrderByDescending(x => x.ReadingDate).FirstOrDefaultAsync();
        if (last != null && (reading.ReadingDate <= last.ReadingDate || reading.MeterValue < last.MeterValue))
            throw new ArgumentException("Reading date must follow the last reading and meter value cannot decrease.");
        context.Readings.Add(reading); await context.SaveChangesAsync(); await tx.CommitAsync(); return reading;
    }

    public async Task RemoveCustomerAsync(int id)
    {
        await using var tx = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var customer = await context.Customers.SingleOrDefaultAsync(x => x.CustomerId == id)
            ?? throw new KeyNotFoundException("Customer not found.");
        if (await context.Accounts.AnyAsync(x => x.CustomerId == id) || await context.Users.AnyAsync(x => x.CustomerId == id))
            throw new InvalidOperationException("Cannot remove a customer with linked water accounts or a sign-in account.");
        context.Customers.Remove(customer);
        await context.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task RemoveAccountAsync(int id)
    {
        await using var tx = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var account = await context.Accounts.SingleOrDefaultAsync(x => x.AccountId == id)
            ?? throw new KeyNotFoundException("Account not found.");
        if (await context.Readings.AnyAsync(x => x.AccountId == id) || await context.Bills.AnyAsync(x => x.AccountId == id))
            throw new InvalidOperationException("Cannot remove an account with recorded readings or bills.");
        context.Accounts.Remove(account);
        await context.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
