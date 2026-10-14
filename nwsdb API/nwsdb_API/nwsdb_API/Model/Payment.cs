namespace nwsdb_API.Model;

public class Payment
{
    public int PaymentId { get; set; }
    public Guid AttemptId { get; set; }
    public int BillId { get; set; }
    public string ReceiptNumber { get; set; } = "";
    public decimal Amount { get; set; }
    public string BankReference { get; set; } = "";
    public DateTime PaidAt { get; set; }
    public Bill Bill { get; set; } = null!;
}
