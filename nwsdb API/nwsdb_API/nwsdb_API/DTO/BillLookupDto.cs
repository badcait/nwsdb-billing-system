using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class BillLookupDto
{
    public int BillId { get; set; }
    [Required, StringLength(200)]
    public string AccountNumber { get; set; } = "";
    [Required, StringLength(200)]
    public string MaskedCustomerName { get; set; } = "";
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public bool IsProcessing { get; set; }
}
