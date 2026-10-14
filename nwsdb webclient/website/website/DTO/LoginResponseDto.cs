using System.ComponentModel.DataAnnotations;
using website.Model;

namespace website.DTO;

public class LoginResponseDto
{
    [Required, StringLength(200)]
    public string AccessToken { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public UserRole Role { get; set; }
    public int UserId { get; set; }
    public int? PartnerId { get; set; }
}
