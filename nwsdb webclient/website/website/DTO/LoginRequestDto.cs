using System.ComponentModel.DataAnnotations;
using website.Model;

namespace website.DTO;

public class LoginRequestDto
{
    [Required, StringLength(200)]
    public string Username { get; set; } = "";

    [Required, StringLength(256)]
    public string Password { get; set; } = "";
}
