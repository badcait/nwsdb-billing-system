using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class LoginRequestDto
{
    [Required, StringLength(200)]
    public string Username { get; set; } = "";
    [Required, StringLength(256)]
    public string Password { get; set; } = "";
}
