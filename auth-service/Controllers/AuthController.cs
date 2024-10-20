using YCompanyClaimsApi.Models;
using YCompanyClaimsApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace YCompanyClaimsApi.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

        [HttpGet("by-role/{role}")]
        // [Authorize(Roles = "ClaimsManager")] // Adjust the authorization as needed
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string role)
        {
            var users = await _userService.GetUsersByRoleAsync(role);
            return Ok(users);
        }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _userService.AuthenticateAsync(model.Username, model.Password);
        if (user == null)
            return Unauthorized();

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

 private string GenerateJwtToken(User user)
{
     if (user == null)
    {
        throw new ArgumentNullException(nameof(user));
    }

    if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Role))
    {
        throw new ArgumentException("User must have a username and role.");
    }
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? 
                _configuration["Jwt:Secret"];

    if (string.IsNullOrEmpty(jwtSecret))
    {
        throw new InvalidOperationException("JWT secret is not configured in environment variables or app settings.");
    }
    var key = Encoding.ASCII.GetBytes(jwtSecret);

        var claims = new List<System.Security.Claims.Claim>
    {
        new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(ClaimTypes.Name, user.Username),
            new System.Security.Claims.Claim(ClaimTypes.Role, user.Role)
    };

    if (user.WorkshopId.HasValue)
    {
        claims.Add(new System.Security.Claims.Claim("WorkshopId", user.WorkshopId.Value.ToString()));
    }
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    


    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
}


}