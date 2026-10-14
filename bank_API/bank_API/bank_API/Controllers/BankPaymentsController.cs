using System.Security.Claims;
using bank_API.DTO;
using bank_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace bank_API.Controllers;

[ApiController, Route("api/bank/payments"), Authorize]
public class BankPaymentsController(BankPaymentService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BankPaymentResultDto>> AuthorizeAsync(BankPaymentRequestDto request) =>
        Ok(await service.AuthorizeAsync(request, User.FindFirstValue(ClaimTypes.NameIdentifier)!));
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BankPaymentResultDto>> GetResultAsync(Guid id) =>
        Ok(await service.GetResultAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!));
}
