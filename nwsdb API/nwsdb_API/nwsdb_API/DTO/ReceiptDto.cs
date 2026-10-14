using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class ReceiptDto
{
    [Required, StringLength(200)]
    public string ReceiptNumber { get; set; } = "";
    [Required, StringLength(200)]
    public string AccountNumber { get; set; } = "";
    public int BillId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    [Required, StringLength(200)]
    public string BankReference { get; set; } = "";
}
