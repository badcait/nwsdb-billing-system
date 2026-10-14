using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
namespace bank_API.Services;

public class ServiceAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder, IConfiguration config)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var expected = config["ServiceAuthentication:Key"];
        var supplied = Request.Headers["X-Service-Key"].ToString();
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(supplied) ||
            !CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(expected)), SHA256.HashData(Encoding.UTF8.GetBytes(supplied))))
            return Task.FromResult(AuthenticateResult.Fail("Invalid service credential."));
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "nwsdb-service")], Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
    }
}
