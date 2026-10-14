using Microsoft.EntityFrameworkCore;
using nwsdb_API.Model;
namespace nwsdb_API.Data;
public class BillRepository(AppDbContext context)
{
    public Task<List<MeterReading>> GetReadingsAsync(int accountId) => context.Readings.AsNoTracking().Where(x => x.AccountId == accountId).OrderBy(x => x.ReadingDate).ToListAsync();
    public Task<List<Bill>> GetBillsAsync(int accountId) => context.Bills.AsNoTracking().Include(x => x.Payment).Include(x => x.Attempts)
        .Where(x => x.AccountId == accountId).OrderByDescending(x => x.PeriodEnd).ToListAsync();
    public async Task<Bill> AddBillAsync(Bill bill)
    {
        if (await context.Bills.AnyAsync(x => x.StartReadingId == bill.StartReadingId || x.EndReadingId == bill.EndReadingId))
            throw new InvalidOperationException("These readings have already been billed.");
        context.Bills.Add(bill); await context.SaveChangesAsync(); return bill;
    }
}
