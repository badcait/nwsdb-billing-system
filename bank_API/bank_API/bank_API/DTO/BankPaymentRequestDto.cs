using System.ComponentModel.DataAnnotations;
using bank_API.Model;

namespace bank_API.DTO;

public class BankPaymentRequestDto
{
    public Guid AttemptId { get; set; }
    public decimal Amount { get; set; }
    [Required, StringLength(200)]
    public string Currency { get; set; } = "";
    [Required, StringLength(200)]
    public string MockPaymentToken { get; set; } = "";
}
