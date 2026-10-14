using AutoMapper;
using bank_API.Data;
using bank_API.DTO;
using bank_API.Model;
namespace bank_API.Services;

public class BankPaymentService(BankTransactionRepository repo, IMapper mapper)
{
    public async Task<BankPaymentResultDto> AuthorizeAsync(BankPaymentRequestDto request, string serviceId)
    {
        if (request.AttemptId == Guid.Empty || request.Amount <= 0 || request.Amount > 999999999 ||
            decimal.Round(request.Amount, 2) != request.Amount || request.Currency != "LKR" ||
            request.MockPaymentToken is not ("demo-approve" or "demo-decline"))
            throw new ArgumentException("Use a valid reference, positive two-decimal LKR amount and demo-approve or demo-decline.");
        var stored = await repo.FindAsync(request.AttemptId, serviceId);
        if (stored != null)
        {
            if (stored.Amount != request.Amount || stored.Currency != request.Currency || stored.MockPaymentToken != request.MockPaymentToken)
                throw new InvalidOperationException("The reference was already used with different payment details.");
            return mapper.Map<BankPaymentResultDto>(stored);
        }
        var transaction = new BankTransaction
        {
            AttemptId = request.AttemptId, ServiceClientId = serviceId, Amount = request.Amount,
            Currency = request.Currency, MockPaymentToken = request.MockPaymentToken,
            Outcome = request.MockPaymentToken == "demo-approve" ? BankOutcome.Approved : BankOutcome.Declined,
            BankReference = "BANK-" + Guid.NewGuid().ToString("N"), ProcessedAt = DateTime.UtcNow
        };
        return mapper.Map<BankPaymentResultDto>(await repo.SaveOutcomeAsync(transaction));
    }
    public async Task<BankPaymentResultDto> GetResultAsync(Guid id, string serviceId) =>
        mapper.Map<BankPaymentResultDto>(await repo.FindAsync(id, serviceId) ?? throw new KeyNotFoundException("Payment not found."));
}
