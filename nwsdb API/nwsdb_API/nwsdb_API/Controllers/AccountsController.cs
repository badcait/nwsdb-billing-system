using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nwsdb_API.DTO;
using nwsdb_API.Model;
using nwsdb_API.Services;
namespace nwsdb_API.Controllers;
[ApiController, Authorize]
public class AccountsController(AccountService service) : ControllerBase
{
    [HttpGet("api/me/accounts"), Authorize(Roles = "Customer")]
    public async Task<ActionResult<AccountSummaryDto[]>> GetMineAsync() => Ok(await service.GetMineAsync(CallerContext.From(User)));
    [HttpGet("api/me/accounts/{id:int}/readings"), Authorize(Roles = "Customer,Staff")]
    public async Task<ActionResult<UsageReadDto[]>> GetReadingsAsync(int id) => Ok(await service.GetReadingsAsync(id, CallerContext.From(User)));
    [HttpGet("api/partner/accounts/{number}/bills"), Authorize(Roles = "PartnerCashier")]
    public async Task<ActionResult<BillLookupDto[]>> LookupAsync(string number) => Ok(await service.LookupAsync(number, CallerContext.From(User)));
    [HttpGet("api/customers"), Authorize(Roles = "Staff")]
    public async Task<ActionResult<CustomerReadDto[]>> SearchCustomersAsync(string search = "", int page = 1) => Ok(await service.SearchCustomersAsync(search, page));
    [HttpGet("api/accounts"), Authorize(Roles = "Staff")]
    public async Task<ActionResult<AccountReadDto[]>> SearchAsync(string search = "", int page = 1) => Ok(await service.SearchAsync(search, page));
    [HttpPost("api/customers"), Authorize(Roles = "Staff")]
    public async Task<ActionResult<CustomerReadDto>> CreateCustomerAsync(CreateCustomerDto request) { var r = await service.CreateCustomerAsync(request); return StatusCode(201, r); }
    [HttpPost("api/accounts"), Authorize(Roles = "Staff")]
    public async Task<ActionResult<AccountReadDto>> CreateAccountAsync(CreateAccountDto request) { var r = await service.CreateAccountAsync(request); return StatusCode(201, r); }
    [HttpPost("api/accounts/{id:int}/readings"), Authorize(Roles = "Staff")]
    public async Task<ActionResult<UsageReadDto>> RecordReadingAsync(int id, CreateReadingDto request) => StatusCode(201, await service.RecordReadingAsync(id, request, CallerContext.From(User)));

    [HttpDelete("api/customers/{id:int}"), Authorize(Roles = "Staff")]
    public async Task<IActionResult> RemoveCustomerAsync(int id)
    {
        await service.RemoveCustomerAsync(id);
        return NoContent();
    }
    [HttpDelete("api/accounts/{id:int}"), Authorize(Roles = "Staff")]
    public async Task<IActionResult> RemoveAccountAsync(int id)
    {
        await service.RemoveAccountAsync(id);
        return NoContent();
    }
}
