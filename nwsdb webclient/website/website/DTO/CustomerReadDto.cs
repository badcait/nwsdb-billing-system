using System.ComponentModel.DataAnnotations;
using website.Model;

namespace website.DTO;

public class CustomerReadDto
{
    public int CustomerId { get; set; }

    [Required, StringLength(200)]
    public string FullName { get; set; } = "";

    [Required, StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required, StringLength(200)]
    public string Phone { get; set; } = "";
}
