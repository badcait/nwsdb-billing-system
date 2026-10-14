using System.ComponentModel.DataAnnotations;
using bank_API.Model;

namespace bank_API.DTO;

public class BankPaymentResultDto
{
    public Guid AttemptId { get; set; }
    public decimal Amount { get; set; }
    [Required, StringLength(200)]
    public string Currency { get; set; } = "";
    public BankOutcome Outcome { get; set; }
    [Required, StringLength(200)]
    public string BankReference { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
}
