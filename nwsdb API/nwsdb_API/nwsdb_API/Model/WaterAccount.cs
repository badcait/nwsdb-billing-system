namespace nwsdb_API.Model;

public class WaterAccount
{
    public int AccountId { get; set; }
    public int CustomerId { get; set; }
    public string AccountNumber { get; set; } = "";
    public string ServiceAddress { get; set; } = "";
    public bool IsActive { get; set; }
    public Customer Customer { get; set; } = null!;
    public List<MeterReading> Readings { get; set; } = new();
    public List<Bill> Bills { get; set; } = new();
}
