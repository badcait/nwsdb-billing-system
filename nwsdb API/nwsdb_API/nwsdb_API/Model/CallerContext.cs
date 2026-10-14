using System.Security.Claims;
namespace nwsdb_API.Model;
public class CallerContext
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
    public int? CustomerId { get; set; }
    public int? PartnerId { get; set; }
    public static CallerContext From(ClaimsPrincipal user) => new()
    {
        UserId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!),
        Role = Enum.Parse<UserRole>(user.FindFirstValue(ClaimTypes.Role)!),
        CustomerId = int.TryParse(user.FindFirstValue("customerId"), out var c) ? c : null,
        PartnerId = int.TryParse(user.FindFirstValue("partnerId"), out var p) ? p : null
    };
}
