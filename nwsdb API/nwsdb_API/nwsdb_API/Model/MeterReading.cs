namespace nwsdb_API.Model;

public class MeterReading
{
    public int ReadingId { get; set; }
    public int AccountId { get; set; }
    public DateTime ReadingDate { get; set; }
    public decimal MeterValue { get; set; }
}
