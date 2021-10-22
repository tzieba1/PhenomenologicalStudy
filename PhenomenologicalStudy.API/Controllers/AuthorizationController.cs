using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PhenomenologicalStudy.API.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using PhenomenologicalStudy.API.Models.Authentication.DataTransfer.Response;
using PhenomenologicalStudy.API.Models;

namespace PhenomenologicalStudy.API.Controllers
{

  [Route("api/[controller]")] // api/authorization
  [ApiController]
  public class AuthorizationController : ControllerBase
  {
    private readonly UserManager<User> _userManager;
    private readonly JwtConfiguration _jwtConfig;

    public AuthorizationController(UserManager<User> userManager, IOptionsMonitor<JwtConfiguration> optionsMonitor)
    {
      _userManager = userManager;
      _jwtConfig = optionsMonitor.CurrentValue; // Inject appsettings.json into this controller.
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] Models.Authentication.DataTransfer.Request.Register user)
    {
      if(ModelState.IsValid)
      {
        // Check email already registered
        if (await _userManager.FindByEmailAsync(user.Email) != null)
        {
          return BadRequest(new Register()
          {
            Errors = new List<string>() { "Email already in use" },
            Success = false
          });
        }

        // Await registration and check for success
        User newUser = new() { Email = user.Email, UserName = user.Email, FirstName = user.FirstName, LastName = user.LastName };
        IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
        if (!result.Succeeded)  // Registration failed
        {
          return BadRequest(new Register()
          {
            Errors = result.Errors.Select(x => x.Description).ToList(),
            Success = false
          });
        }
        else  // Authentication success
          return Ok(new Register() { Success = true, Token = GenerateJwtToken(newUser) });
      }

      return BadRequest(new Register()
      {
        Errors = new List<string>() { "Invalid payload" },
        Success = false
      });
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] Models.Authentication.DataTransfer.Request.Login user)
    {
      if (ModelState.IsValid)
      {
        User existingUser = await _userManager.FindByEmailAsync(user.Email);
        // Check user exists
        if (existingUser == null)
        {
          return BadRequest(new Login()
          {
            Errors = new List<string>() { "Invalid login request" },
            Success = false
          });
        }

        if(!(await _userManager.CheckPasswordAsync(existingUser, user.Password)))
        {
          return BadRequest(new Login()
          {
            Errors = new List<string>() { "Invalid password" },
            Success = false
          });
        }
        else
          return Ok(new Register() { Success = true, Token = GenerateJwtToken(existingUser) });
      }

      return BadRequest(new Login()
      {
        Errors = new List<string>() { "Invalid payload" },
        Success = false
      });
    }

    /// <summary>
    /// Use a configuration secret to generate a JWT token with a SecurityTokenDescriptor.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private string GenerateJwtToken(User user)
    {
      JwtSecurityTokenHandler jwtTokenHandler = new();
      byte[] key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
      // SecurityTokenDescriptor works to register claims to a JwtPayload (e.g. Subject -> sub, Expires -> exp)
      SecurityTokenDescriptor tokenDescriptor = new() { 
        Subject = new ClaimsIdentity(new [] { 
          new Claim("Id", user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.Email, user.Email),
          new Claim(JwtRegisteredClaimNames.Sub, user.FirstName),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Needed to use token refresh functionality supported by JWT
        }),
        Expires = DateTime.UtcNow.AddSeconds(30), // Only 30 seconds for demo purposes (use ~5-10 mins in production)
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      // Use security descriptor to generate JWT token
      SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
      return jwtTokenHandler.WriteToken(token);
    }
  }
}
