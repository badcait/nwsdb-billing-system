using AutoMapper;
using nwsdb_API.Data;
using nwsdb_API.DTO;
using nwsdb_API.Model;
namespace nwsdb_API.Services;
public class PaymentService(PaymentRepository repo, BankGatewayClient bank, IMapper mapper, ILogger<PaymentService> logger)
{
    public async Task<PaymentStatusDto> SubmitAsync(CreatePaymentDto request, CallerContext caller)
    {
        if (caller.Role != UserRole.PartnerCashier || caller.PartnerId == null) throw new UnauthorizedAccessException("Partner access required.");
        if (request.ClientReference == Guid.Empty || request.BillId <= 0 || request.Amount <= 0 || request.Amount > 999999999 ||
            decimal.Round(request.Amount, 2) != request.Amount || request.MockPaymentToken is not ("demo-approve" or "demo-decline"))
            throw new ArgumentException("Provide a bill, full two-decimal amount, unique reference and valid demo token.");
        var reservation = await repo.ReserveAsync(request, caller.PartnerId.Value);
        if (reservation.Kind == ReservationKind.Conflict) throw new PaymentConflictException(reservation.ConflictCode!, reservation.NoReservation ? "BillUnavailable" : "ReferenceMismatch");
        if (reservation.Kind == ReservationKind.Replay) return mapper.Map<PaymentStatusDto>(reservation.Attempt);
        return await ResolveAsync(reservation.Attempt!, caller);
    }
    public async Task<PaymentStatusDto> GetStatusAsync(Guid id, CallerContext caller) => mapper.Map<PaymentStatusDto>(await repo.FindAuthorizedAsync(id, caller));
    public Task<PaymentStatusDto> ReadBackAsync(Guid id, CallerContext caller) => GetStatusAsync(id, caller);
    public async Task<PaymentStatusDto> ResolveAsync(PaymentAttempt attempt, CallerContext caller)
    {
        try
        {
            var result = await bank.AuthorizeAsync(new BankPaymentRequestDto { AttemptId = attempt.AttemptId, Amount = attempt.Amount,
                Currency = attempt.Currency, MockPaymentToken = attempt.MockPaymentToken });
            return await FinalizeAsync(attempt, result, caller);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Payment outcome needs read-back for {AttemptId}", attempt.AttemptId);
            return await ReadBackAsync(attempt.AttemptId, caller);
        }
    }
    public async Task<PaymentStatusDto> FinalizeAsync(PaymentAttempt attempt, BankPaymentResultDto result, CallerContext caller)
    {
        if (result.AttemptId != attempt.AttemptId || result.Amount != attempt.Amount || result.Currency != attempt.Currency ||
            string.IsNullOrWhiteSpace(result.BankReference) || result.BankReference.Length > 200 || result.ProcessedAt == default || !Enum.IsDefined(result.Outcome))
            throw new HttpRequestException("Bank response did not match the saved attempt.");
        if (result.Outcome == BankOutcome.Approved) await repo.CompleteAsync(attempt.AttemptId, result);
        else await repo.DeclineAsync(attempt.AttemptId, result);
        return await ReadBackAsync(attempt.AttemptId, caller);
    }
    public async Task<PaymentStatusDto> ReconcileAsync(Guid id, CallerContext caller)
    {
        var attempt = await repo.FindAuthorizedAsync(id, caller);
        if (attempt.Status != AttemptStatus.Pending) return mapper.Map<PaymentStatusDto>(attempt);
        try
        {
            var result = await bank.GetResultAsync(id);
            return result == null ? await ResolveAsync(attempt, caller) : await FinalizeAsync(attempt, result, caller);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Reconciliation needs read-back for {AttemptId}", id);
            return await ReadBackAsync(id, caller);
        }
    }
    public async Task<PaymentStatusDto[]> GetHistoryAsync(CallerContext caller) => mapper.Map<PaymentStatusDto[]>(await repo.GetHistoryAsync(caller));
    public async Task<ReceiptDto> GetReceiptAsync(string number, CallerContext caller) => mapper.Map<ReceiptDto>(await repo.FindReceiptAuthorizedAsync(number, caller));
    public async Task<ReceiptDto[]> GetCustomerPaymentsAsync(CallerContext caller) => mapper.Map<ReceiptDto[]>(await repo.GetCustomerPaymentsAsync(caller.CustomerId!.Value));
}
