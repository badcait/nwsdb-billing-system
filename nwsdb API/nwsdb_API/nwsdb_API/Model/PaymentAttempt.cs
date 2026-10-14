namespace nwsdb_API.Model;

public class PaymentAttempt
{
    public Guid AttemptId { get; set; }
    public int PartnerId { get; set; }
    public int BillId { get; set; }
    public Guid ClientReference { get; set; }
    public string MockPaymentToken { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "";
    public AttemptStatus Status { get; set; }
    public string? BankReference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Bill Bill { get; set; } = null!;
    public Payment? Payment { get; set; }
}
