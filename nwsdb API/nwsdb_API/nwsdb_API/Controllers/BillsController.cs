using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nwsdb_API.DTO;
using nwsdb_API.Model;
using nwsdb_API.Services;
namespace nwsdb_API.Controllers;
[ApiController, Authorize]
public class BillsController(BillService service) : ControllerBase
{
    [HttpGet("api/me/accounts/{id:int}/bills"), Authorize(Roles = "Customer,Staff")]
    public async Task<ActionResult<BillReadDto[]>> GetBillsAsync(int id) => Ok(await service.GetBillsAsync(id, CallerContext.From(User)));
    [HttpPost("api/accounts/{id:int}/bills"), Authorize(Roles = "Staff")]
    public async Task<ActionResult<BillReadDto>> GenerateAsync(int id, GenerateBillDto request) => StatusCode(201, await service.GenerateAsync(id, request, CallerContext.From(User)));
}
