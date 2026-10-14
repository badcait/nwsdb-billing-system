using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class BillReadDto
{
    public int BillId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Units { get; set; }
    public decimal Amount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public bool IsProcessing { get; set; }
}
