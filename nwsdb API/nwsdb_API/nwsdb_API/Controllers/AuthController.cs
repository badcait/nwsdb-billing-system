using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using nwsdb_API.DTO;
using nwsdb_API.Services;
namespace nwsdb_API.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(AuthService service) : ControllerBase
{
    [HttpPost("login"), EnableRateLimiting("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var result = await service.LoginAsync(request);
        return result == null ? Unauthorized(new ProblemDetails { Title = "Invalid username or password.", Status = 401 }) : Ok(result);
    }
}
