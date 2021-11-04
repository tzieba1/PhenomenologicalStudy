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
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using PhenomenologicalStudy.API.Authentication.Request;
using PhenomenologicalStudy.API.Authentication.Response;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;

namespace PhenomenologicalStudy.API.Controllers
{

  [Route("api/[controller]")] // api/authentication
  [ApiController]
  public class AuthenticationController : ControllerBase
  {
    private readonly UserManager<User> _userManager;
    private readonly JwtConfiguration _jwtConfig;
    private readonly TokenValidationParameters _tokenValidationParams;
    private readonly PhenomenologicalStudyContext _userDbContext;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IEmailSender _emailSender;

    public AuthenticationController(
      UserManager<User> userManager,
      IConfiguration config,
      TokenValidationParameters tokenValidationParams,
      PhenomenologicalStudyContext userDbContext,
      ILogger<AuthenticationController> logger,
      IEmailSender emailSender)
    {
      _userManager = userManager;                                           // Manages users for registration(creation)/login and validation
      _jwtConfig = config.GetSection("JwtConfig").Get<JwtConfiguration>();  // Inject secret JWT configuration into this controller
      _userDbContext = userDbContext;                                       // Needed to handle RefreshTokens on the database
      _tokenValidationParams = tokenValidationParams;                       // For generating and validating JWTs
      _logger = logger;                                                     // Generate logs for this controller
      _emailSender = emailSender;                                           // For registration confirmation emails
    }

    /// <summary>
    /// Endpoint for user to register and recieve an email confirmation link.
    /// </summary>
    /// <param name="user">Candidate user being registered</param>
    /// <returns>AuthenticationResult with status of email confirmation, otherwise BadRequest</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest user)
    {
      if (ModelState.IsValid)
      {
        // Check email already registered
        if (await _userManager.FindByEmailAsync(user.Email) != null)
        {
          return BadRequest(new AuthenticationResult()
          {
            Errors = new List<string>() { "Email already in use" },
            Success = false
          });
        }

        // Await registration and check for success
        User newUser = new() { 
          Email = user.Email, 
          UserName = user.Email, 
          FirstName = user.FirstName, 
          LastName = user.LastName 
        };
        IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
        if (!result.Succeeded)  // Registration failed
        {
          return BadRequest(new AuthenticationResult()
          {
            Errors = result.Errors.Select(x => x.Description).ToList(),
            Success = false
          });
        }
        else  // Send confirmation email
        {
          User createdUser = await _userManager.FindByEmailAsync(newUser.Email); // Retrieve the created user to get their Id
          _logger.LogInformation("User created a new account with password.");

          // Generate a confirmation token to securely confirm an email address for a successfully registered user
          string code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
          code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

          // Send callback for route in this controller to ConfirmEmail
          //string callbackUrl = Url.Action("ConfirmEmail", "Authentication", new { userId = createdUser.Id.ToString(), code = code });
          var urlBuilder = new UriBuilder() { 
            Scheme = "https",
            Host = "localhost",
            Port = 5001,
            Path = "/api/Authentication/ConfirmEmail",
            Query = $"?userId={createdUser.Id}&code={code}"
          };

          await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email",
              $"Please confirm your account by <a href='{urlBuilder}'>clicking here</a>.");

          if (_userManager.Options.SignIn.RequireConfirmedAccount)
          {
            return Ok(
              new AuthenticationResult()
              {
                StatusMessages = new List<string>() { "Confirmation email sent (check spam folder)." },
                Success = true
              }
            );
          }
          else
            return Ok();
        }
      }

      return BadRequest(new AuthenticationResult()
      {
        Errors = new List<string>() { "Invalid payload" },
        Success = false
      });
    }

    /// <summary>
    /// Endpoint for user to confirm their email (only reachable by link sent to registerating email with security token attached).
    /// </summary>
    /// <param name="userId">Guid of user whos email is being confirmed</param>
    /// <param name="code">Security token generated by _userManager</param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery]string userId, [FromQuery] string code)
    {
      //string userId = confirmation.UserId;
      //string code = confirmation.Code;
      if (userId == null || code == null)
      {
        return BadRequest(new AuthenticationResult()
        {
          Errors = new List<string>() { "Ivalid email confirmation request." },
          Success = false
        });
      }

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return NotFound($"Unable to load user with ID '{userId}'.");
      }

      code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
      var result = await _userManager.ConfirmEmailAsync(user, code);
      var statusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

      var confirmResult = new AuthenticationResult(await GenerateJwt(user), true);
      confirmResult.StatusMessages = new List<string>() { statusMessage };
      return Ok(confirmResult);
    }

    /// <summary>
    /// Endpoint for user to login and recieve JWT including both token and refresh token properties.
    /// </summary>
    /// <param name="user">Login request POCO representing user credentials</param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest user)
    {
      if (ModelState.IsValid)
      {
        User existingUser = await _userManager.FindByEmailAsync(user.Email);
        // Check user exists
        if (existingUser == null)
        {
          return NotFound(new AuthenticationResult()
          {
            Errors = new List<string>() { "User not found" },
            Success = false
          });
        }

        // Check user has not confirmed their email
        if (!existingUser.EmailConfirmed)
        {
          return NotFound(new AuthenticationResult()
          {
            Errors = new List<string>() { "Email has not been confirmed." },
            Success = false
          });
        }

        // Check password matches after hash/salt/encryption
        if (!(await _userManager.CheckPasswordAsync(existingUser, user.Password)))
        {
          return BadRequest(new AuthenticationResult()
          {
            Errors = new List<string>() { "Invalid password" },
            Success = false
          });
        }
        else
          return Ok(new AuthenticationResult(await GenerateJwt(existingUser), true));
      }

      return BadRequest(new AuthenticationResult()
      {
        Errors = new List<string>() { "Invalid payload" },
        Success = false
      });
    }

    /// <summary>
    /// Endpoint used to refresh JWTs for users making requests to other API endpoints
    /// </summary>
    /// <param name="token"></param>
    /// <returns>BadRequest if Jwt canot be refreshed, otherwise refreshed Jwt</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] Jwt token)
    {
      if (ModelState.IsValid)
      {
        Jwt result = await ValidateAndGenerateToken(token);
        if (result == null)
        {
          return BadRequest(new AuthenticationResult()
          {
            Errors = new List<string>() { "Invalid tokens" },
            Success = false
          });
        }
        return Ok(result);
      }

      return BadRequest(new AuthenticationResult()
      {
        Errors = new List<string>() { "Invalid payload" },
        Success = false
      });
    }

    /// <summary>
    /// Use a configuration private key to generate a JWT token with a SecurityTokenDescriptor.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    async private Task<Jwt> GenerateJwt(User user)
    {
      JwtSecurityTokenHandler jwtTokenHandler = new();
      byte[] key = Encoding.UTF8.GetBytes(_jwtConfig.PrivateKey);

      // SecurityTokenDescriptor works to register claims to a JwtPayload (e.g. Subject -> sub, Expires -> exp)
      SecurityTokenDescriptor tokenDescriptor = new()
      {
        Subject = new ClaimsIdentity(new[] {
          new Claim("Id", user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.Email, user.Email),             // Email claim of user generating Jwt
          new Claim(JwtRegisteredClaimNames.Sub, user.FirstName),           // Sub claim identifies principal Subject of the JWT
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Enables JWT refresh token functionality
        }),
        Expires = DateTime.UtcNow.AddSeconds(30), // Only 30 seconds for demo purposes (use ~5-10 mins in production)
        SigningCredentials = new SigningCredentials(
          _tokenValidationParams.IssuerSigningKey,
          SecurityAlgorithms.HmacSha256Signature
        ),
        Audience = _jwtConfig.ValidAudience,
        Issuer = _jwtConfig.ValidIssuer
      };

      // Use security descriptor to generate JWT token
      SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);

      // Refreshes the JWT token
      RefreshToken refreshToken = new()
      {
        JwtId = new Guid(token.Id),
        IsUsed = false,
        IsRevoked = false,
        UserId = user.Id,
        CreatedDate = DateTimeOffset.UtcNow,
        ExpiryDate = DateTimeOffset.UtcNow.AddMonths(6),
        Token = RandomString(35) + Guid.NewGuid()
      };

      // Save refreshToken to database
      await _userDbContext.RefreshTokens.AddAsync(refreshToken);
      await _userDbContext.SaveChangesAsync();

      //return jwtTokenHandler.WriteToken(token);
      return new AuthenticationResult()
      {
        Token = jwtTokenHandler.WriteToken(token),
        Success = true,
        RefreshToken = refreshToken.Token,
      };
    }

    /// <summary>
    /// Helper for validating tokens that are being generated during token refresh.
    /// </summary>
    /// <param name="token">Token from request body being refreshed.</param>
    /// <returns>Jwt with Token and RefreshToken properties</returns>
    private async Task<Jwt> ValidateAndGenerateToken(Jwt token)
    {
      JwtSecurityTokenHandler jwtTokenHandler = new();  // Handles verification of a Jwt
      try
      {
        // 1. Validate JWT token format to pass on for encryption algorithm validation
        ClaimsPrincipal tokenCandidate = jwtTokenHandler.ValidateToken(token.Token, _tokenValidationParams, out SecurityToken validatedToken);

        // 2. Validate encryption algorithm
        if (validatedToken is JwtSecurityToken jwtSecurityToken)
        {
          if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            return null;
        }

        // 3. Validate expiry date is after now (no need to refresh)
        long utcExpiryDate = long.Parse(tokenCandidate.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
        DateTimeOffset expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
        if (expiryDate > DateTime.UtcNow)
        {
          return new AuthenticationResult()
          {
            Success = false,
            Errors = new List<string>() { "Token has not expired" }
          };
        }

        // Attempt to find stored RefreshToken entry in database with Token matching token.RefreshToken 
        RefreshToken storedToken = await _userDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token.RefreshToken);

        // 4. Validate stored RefreshToken exists
        if (storedToken == null)
        {
          return new AuthenticationResult()
          {
            Success = false,
            Errors = new List<string>() { "Token does not exist" }
          };
        }

        // 5. Validate stored RefreshToken has not been used
        if (storedToken.IsUsed)
        {
          return new AuthenticationResult()
          {
            Success = false,
            Errors = new List<string>() { "Token has been used" }
          };
        }

        // 6. Validate stored RefreshToken has not been revoked
        if (storedToken.IsRevoked)
        {
          return new AuthenticationResult()
          {
            Success = false,
            Errors = new List<string>() { "Token has been revoked" }
          };
        }

        // 7. Validate token claim Id from ClaimsPrincipal object (tokenCandidate) returned from 1st validation
        string jti = tokenCandidate.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        if (storedToken.JwtId.ToString() != jti)
        {
          return new AuthenticationResult()
          {
            Success = false,
            Errors = new List<string>() { "Token does not match" }
          };
        }

        storedToken.IsUsed = true;                        // Set stored RefreshToken as used
        _userDbContext.RefreshTokens.Update(storedToken); // Update stored RefreshToken
        await _userDbContext.SaveChangesAsync();          // Save changes

        // Generate and return new Jwt for the UserId associated to stored RefreshToken
        return await GenerateJwt(await _userManager.FindByIdAsync(storedToken.UserId.ToString()));
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
        {
          return new AuthenticationResult()
          {
            Success = false,
            Errors = new List<string>() { "Token has expired please login again." }
          };
        }
        else
        {
          return new AuthenticationResult()
          {
            Success = false,
            Errors = new List<string>() { "Something went wrong." }
          };
        }
      }
    }

    /// <summary>
    /// Convert UnixTimeStamp to DateTime.
    /// </summary>
    /// <param name="unixTimeStamp">UnixTimeStamp to bet converted.</param>
    /// <returns>DateTime</returns>
    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
      var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      return dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
    }

    /// <summary>
    /// Use alpha-numeric characters to generate a random string parametrized by character length.
    /// </summary>
    /// <param name="length">Length of random string returned</param>
    /// <returns>Random alpha-numeric string</returns>
    private static string RandomString(int length)
    {
      Random random = new();
      string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
    }
  }
}