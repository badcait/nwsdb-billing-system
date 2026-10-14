namespace bank_API.Model;

public class BankTransaction
{
    public Guid AttemptId { get; set; }
    public string ServiceClientId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "";
    public string MockPaymentToken { get; set; } = "";
    public BankOutcome Outcome { get; set; }
    public string BankReference { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
}
