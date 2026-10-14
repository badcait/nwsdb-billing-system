using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class PaymentStatusDto
{
    public Guid AttemptId { get; set; }
    public Guid ClientReference { get; set; }
    public int BillId { get; set; }
    [Required, StringLength(200)]
    public string AccountNumber { get; set; } = "";
    public decimal Amount { get; set; }
    [Required, StringLength(200)]
    public string Currency { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public AttemptStatus Status { get; set; }
    [Required, StringLength(200)]
    public string Message { get; set; } = "";
    public ReceiptDto? Receipt { get; set; }
}
