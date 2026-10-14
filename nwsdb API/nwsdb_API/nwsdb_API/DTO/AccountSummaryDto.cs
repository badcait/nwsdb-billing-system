using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class AccountSummaryDto
{
    public int AccountId { get; set; }
    [Required, StringLength(200)]
    public string AccountNumber { get; set; } = "";
    public decimal? LatestMeterValue { get; set; }
    public DateTime? LatestReadingDate { get; set; }
    public decimal? LatestConsumption { get; set; }
    public decimal OutstandingAmount { get; set; }
    public bool IsProcessing { get; set; }
}
