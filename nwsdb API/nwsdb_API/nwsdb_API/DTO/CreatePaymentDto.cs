using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class CreatePaymentDto
{
    public int BillId { get; set; }
    public decimal Amount { get; set; }
    public Guid ClientReference { get; set; }
    [Required, StringLength(200)]
    public string MockPaymentToken { get; set; } = "";
}
