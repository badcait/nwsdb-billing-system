using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nwsdb_API.DTO;
using nwsdb_API.Model;
using nwsdb_API.Services;
namespace nwsdb_API.Controllers;
[ApiController, Authorize]
public class PaymentsController(PaymentService service) : ControllerBase
{
    private ActionResult<PaymentStatusDto> StatusResult(PaymentStatusDto result) => StatusCode(result.Status == AttemptStatus.Pending ? 202 : 200, result);
    [HttpPost("api/payments"), Authorize(Roles = "PartnerCashier")]
    public async Task<ActionResult<PaymentStatusDto>> SubmitAsync(CreatePaymentDto request) => StatusResult(await service.SubmitAsync(request, CallerContext.From(User)));
    [HttpGet("api/payments/{id:guid}"), Authorize(Roles = "PartnerCashier,Staff")]
    public async Task<ActionResult<PaymentStatusDto>> GetStatusAsync(Guid id) => Ok(await service.GetStatusAsync(id, CallerContext.From(User)));
    [HttpGet("api/payments"), Authorize(Roles = "PartnerCashier,Staff")]
    public async Task<ActionResult<PaymentStatusDto[]>> GetHistoryAsync() => Ok(await service.GetHistoryAsync(CallerContext.From(User)));
    [HttpPost("api/payments/{id:guid}/reconcile"), Authorize(Roles = "PartnerCashier,Staff")]
    public async Task<ActionResult<PaymentStatusDto>> ReconcileAsync(Guid id) => StatusResult(await service.ReconcileAsync(id, CallerContext.From(User)));
    [HttpGet("api/me/payments"), Authorize(Roles = "Customer")]
    public async Task<ActionResult<ReceiptDto[]>> GetMineAsync() => Ok(await service.GetCustomerPaymentsAsync(CallerContext.From(User)));
    [HttpGet("api/receipts/{number}"), Authorize(Roles = "Customer,PartnerCashier,Staff")]
    public async Task<ActionResult<ReceiptDto>> GetReceiptAsync(string number) => Ok(await service.GetReceiptAsync(number, CallerContext.From(User)));
}
