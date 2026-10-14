using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using nwsdb_API.Data;
using nwsdb_API.DTO;
using nwsdb_API.Model;
namespace nwsdb_API.Services;

public class AuthService(UserRepository repo, IConfiguration config)
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await repo.FindByUsernameAsync(request.Username.Trim());
        if (user == null || !user.IsActive || (user.Role == UserRole.PartnerCashier && user.Partner?.IsActive != true) ||
            new PasswordHasher<AppUser>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return null;
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.UserId.ToString()), new(ClaimTypes.Role, user.Role.ToString()) };
        if (user.CustomerId.HasValue) claims.Add(new("customerId", user.CustomerId.Value.ToString()));
        if (user.PartnerId.HasValue) claims.Add(new("partnerId", user.PartnerId.Value.ToString()));
        var expires = DateTime.UtcNow.AddHours(2);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], claims,
            expires: expires, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new LoginResponseDto { AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expires, Role = user.Role, UserId = user.UserId, PartnerId = user.PartnerId };
    }
}
