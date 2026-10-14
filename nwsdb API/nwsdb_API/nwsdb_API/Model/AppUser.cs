namespace nwsdb_API.Model;

public class AppUser
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public int? CustomerId { get; set; }
    public int? PartnerId { get; set; }
    public Customer? Customer { get; set; }
    public Partner? Partner { get; set; }
}
