namespace nwsdb_API.Model;

public class Bill
{
    public int BillId { get; set; }
    public int AccountId { get; set; }
    public int StartReadingId { get; set; }
    public int EndReadingId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Units { get; set; }
    public decimal UnitRate { get; set; }
    public decimal FixedCharge { get; set; }
    public decimal Amount { get; set; }
    public WaterAccount Account { get; set; } = null!;
    public Payment? Payment { get; set; }
    public List<PaymentAttempt> Attempts { get; set; } = new();
}
