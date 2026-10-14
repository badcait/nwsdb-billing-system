using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class BankPaymentRequestDto
{
    public Guid AttemptId { get; set; }
    public decimal Amount { get; set; }
    [Required, StringLength(200)]
    public string Currency { get; set; } = "";
    [Required, StringLength(200)]
    public string MockPaymentToken { get; set; } = "";
}
