using AutoMapper;
using nwsdb_API.Data;
using nwsdb_API.DTO;
using nwsdb_API.Model;
namespace nwsdb_API.Services;
public class AccountService(AccountRepository repo, IMapper mapper)
{
    public async Task<AccountSummaryDto[]> GetMineAsync(CallerContext caller)
    {
        if (caller.Role != UserRole.Customer || caller.CustomerId == null) throw new UnauthorizedAccessException("Customer access required.");
        return (await repo.GetOwnedAsync(caller.CustomerId.Value)).Select(a =>
        {
            var r = a.Readings.OrderByDescending(x => x.ReadingDate).Take(2).ToArray();
            return new AccountSummaryDto { AccountId = a.AccountId, AccountNumber = a.AccountNumber,
                LatestMeterValue = r.FirstOrDefault()?.MeterValue, LatestReadingDate = r.FirstOrDefault()?.ReadingDate,
                LatestConsumption = r.Length == 2 ? r[0].MeterValue - r[1].MeterValue : null,
                OutstandingAmount = a.Bills.Where(x => x.Payment == null).Sum(x => x.Amount),
                IsProcessing = a.Bills.Any(x => x.Attempts.Any(t => t.Status == AttemptStatus.Pending)) };
        }).ToArray();
    }
    public async Task<WaterAccount> RequireAccountAsync(int id, CallerContext caller)
    {
        var a = await repo.FindAsync(id) ?? throw new KeyNotFoundException("Account not found.");
        if (caller.Role != UserRole.Staff && (caller.Role != UserRole.Customer || a.CustomerId != caller.CustomerId))
            throw new KeyNotFoundException("Account not found.");
        return a;
    }
    public async Task<UsageReadDto[]> GetReadingsAsync(int id, CallerContext caller)
    {
        var a = await RequireAccountAsync(id, caller);
        var readings = a.Readings.OrderBy(x => x.ReadingDate).ToArray();
        return readings.Select((r, i) => new UsageReadDto { ReadingId = r.ReadingId, ReadingDate = r.ReadingDate,
            MeterValue = r.MeterValue, Consumption = i == 0 ? null : r.MeterValue - readings[i - 1].MeterValue }).ToArray();
    }
    public async Task<BillLookupDto[]> LookupAsync(string number, CallerContext caller)
    {
        if (caller.Role != UserRole.PartnerCashier) throw new UnauthorizedAccessException("Partner access required.");
        var a = await repo.FindByNumberAsync(number.Trim());
        if (a == null || !a.IsActive) return [];
        return a.Bills.Where(x => x.Payment == null).Select(x => new BillLookupDto { BillId = x.BillId, AccountNumber = a.AccountNumber,
            MaskedCustomerName = a.Customer.FullName[..1] + "***", PeriodStart = x.PeriodStart, PeriodEnd = x.PeriodEnd,
            DueDate = x.DueDate, Amount = x.Amount, IsProcessing = x.Attempts.Any(t => t.Status == AttemptStatus.Pending) }).ToArray();
    }
    public async Task<UsageReadDto> RecordReadingAsync(int id, CreateReadingDto request, CallerContext caller)
    {
        if (caller.Role != UserRole.Staff) throw new UnauthorizedAccessException("Staff access required.");
        if (request.ReadingDate == default || request.ReadingDate.Date > DateTime.UtcNow.Date || request.MeterValue < 0 || request.MeterValue > 999999999 || decimal.Round(request.MeterValue, 2) != request.MeterValue)
            throw new ArgumentException("Provide a valid reading date and non-negative meter value with at most two decimals.");
        var r = await repo.AddReadingAsync(new MeterReading { AccountId = id, ReadingDate = request.ReadingDate.Date, MeterValue = request.MeterValue });
        return (await GetReadingsAsync(id, caller)).Single(x => x.ReadingId == r.ReadingId);
    }
    public async Task<CustomerReadDto> CreateCustomerAsync(CreateCustomerDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName)) throw new ArgumentException("Customer name is required.");
        return mapper.Map<CustomerReadDto>(await repo.AddCustomerAsync(mapper.Map<Customer>(request)));
    }
    public async Task<AccountReadDto> CreateAccountAsync(CreateAccountDto request)
    {
        if (string.IsNullOrWhiteSpace(request.AccountNumber) || string.IsNullOrWhiteSpace(request.ServiceAddress)) throw new ArgumentException("Account number and address are required.");
        var a = mapper.Map<WaterAccount>(request); a.AccountNumber = a.AccountNumber.Trim(); a.IsActive = true;
        return mapper.Map<AccountReadDto>(await repo.AddAsync(a));
    }
    public async Task<CustomerReadDto[]> SearchCustomersAsync(string search, int page) => mapper.Map<CustomerReadDto[]>(await repo.SearchCustomersAsync((search ?? string.Empty).Trim(), Math.Clamp(page, 1, 1000000)));
    public async Task<AccountReadDto[]> SearchAsync(string search, int page) => mapper.Map<AccountReadDto[]>(await repo.SearchAsync((search ?? string.Empty).Trim(), Math.Clamp(page, 1, 1000000)));

    public Task RemoveCustomerAsync(int id) => repo.RemoveCustomerAsync(id);
    public Task RemoveAccountAsync(int id) => repo.RemoveAccountAsync(id);
}
