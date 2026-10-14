using System.ComponentModel.DataAnnotations;
using website.Model;

namespace website.DTO;

public class CreateAccountDto
{
    public int CustomerId { get; set; }

    [Required, StringLength(200)]
    public string AccountNumber { get; set; } = "";

    [Required, StringLength(200)]
    public string ServiceAddress { get; set; } = "";
}
