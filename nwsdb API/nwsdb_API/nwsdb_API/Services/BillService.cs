using AutoMapper;
using nwsdb_API.Data;
using nwsdb_API.DTO;
using nwsdb_API.Model;
namespace nwsdb_API.Services;
public class BillService(BillRepository repo, AccountService accounts, IMapper mapper, IConfiguration config)
{
    public async Task<BillReadDto[]> GetBillsAsync(int id, CallerContext caller)
    { await accounts.RequireAccountAsync(id, caller); return mapper.Map<BillReadDto[]>(await repo.GetBillsAsync(id)); }
    public async Task<BillReadDto> GenerateAsync(int id, GenerateBillDto request, CallerContext caller)
    {
        if (caller.Role != UserRole.Staff) throw new UnauthorizedAccessException("Staff access required.");
        var a = await accounts.RequireAccountAsync(id, caller);
        if (!a.IsActive) throw new InvalidOperationException("Account is inactive.");
        var readings = await repo.GetReadingsAsync(id);
        var index = readings.FindIndex(x => x.ReadingId == request.StartReadingId);
        if (index < 0 || index + 1 >= readings.Count || readings[index + 1].ReadingId != request.EndReadingId)
            throw new ArgumentException("Select consecutive readings from this account.");
        var start = readings[index]; var end = readings[index + 1];
        if (request.DueDate.Date < end.ReadingDate.Date) throw new ArgumentException("Due date cannot precede the end reading.");
        var rate = config.GetValue<decimal>("Billing:UnitRate"); var charge = config.GetValue<decimal>("Billing:FixedCharge");
        var units = end.MeterValue - start.MeterValue;
        var bill = new Bill { AccountId = id, StartReadingId = start.ReadingId, EndReadingId = end.ReadingId,
            PeriodStart = start.ReadingDate, PeriodEnd = end.ReadingDate, DueDate = request.DueDate.Date,
            Units = units, UnitRate = rate, FixedCharge = charge, Amount = decimal.Round(units * rate + charge, 2) };
        return mapper.Map<BillReadDto>(await repo.AddBillAsync(bill));
    }
}
