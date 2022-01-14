using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PhenomenologicalStudy.API.Configuration;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services
{
  public class AuthService : Interfaces.IAuthService
  {
    private readonly UserManager<User> _userManager;
    private readonly PhenomenologicalStudyContext _db;
    private readonly JwtConfiguration _jwtConfig;
    private readonly TokenValidationParameters _tokenValidationParams;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthService> _logger;
    private readonly IMapper _mapper;
    private readonly RoleManager<Role> _roleManager;
    public AuthService(PhenomenologicalStudyContext db,
                       IConfiguration config, // Includes 'appsettings.json' contents as sections
                       UserManager<User> userManager,
                       TokenValidationParameters tokenValidationParams,
                       IHttpContextAccessor httpContextAccessor,
                       IEmailSender emailSender,
                       ILogger<AuthService> logger,
                       IMapper mapper,
                       RoleManager<Role> roleManager
      )
    {
      _userManager = userManager;                                           // Manages users for registration(creation)/login and validation
      _db = db;                                                             // Manages users for registration(creation)/login and validation                             
      _jwtConfig = config.GetSection("JwtConfig").Get<JwtConfiguration>();  // Inject secret JWT configuration into this controller
      _tokenValidationParams = tokenValidationParams;                       // Generate and validate JWTs using secrets in 'appsettings.json'
      _httpContextAccessor = httpContextAccessor;                           // Needed by GetUserId() to retrieve userId claim from JWT
      _emailSender = emailSender;                                           // For registration confirmation emails
      _logger = logger;                                                     // Generate logs for this controller
      _mapper = mapper;
      _roleManager = roleManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<RefreshTokenDto>> Login(LoginUserDto user)
    {
      try
      {
        User existingUser = await _userManager.FindByEmailAsync(user.Email);
        // Check user exists
        if (existingUser == null)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Messages = new List<string>() { "User not found." },
            Status = HttpStatusCode.NotFound,
            Success = false
          };
        }

        // Check user has not confirmed their email
        if (!existingUser.EmailConfirmed)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Messages = new List<string>() { "Email has not been confirmed." },
            Status = HttpStatusCode.NotFound,
            Success = false
          };
        }

        // Check password matches after hash/salt/encryption
        if (!(await _userManager.CheckPasswordAsync(existingUser, user.Password)))
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Messages = new List<string>() { "Invalid password" },
            Status = HttpStatusCode.BadRequest,
            Success = false
          };
        }
        else
          return await GenerateJwt(existingUser);
      }
      catch (Exception ex)
      {
        return new ServiceResponse<RefreshTokenDto>()
        {
          Messages = new List<string>() { ex.Message },
          Status = HttpStatusCode.InternalServerError,
          Success = false
        };
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> Logout(AuthenticationTokensDto tokens)
    {
      ServiceResponse<Guid> serviceResponse = new();

      // Attempt to find refresh token of user to logout so it can be revoked
      RefreshToken refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == tokens.RefreshJwt);

      // Check that refresh token was not found.
      if (refreshToken == null)
      {
        serviceResponse.Messages.Add("Refresh token not found.");
        serviceResponse.Status = HttpStatusCode.NotFound;
        serviceResponse.Success = false;
        return serviceResponse;
      }

      // Revoke refresh token, save changes, and add message to service response.
      refreshToken.IsRevoked = true;
      await _db.SaveChangesAsync();
      serviceResponse.Messages.Add($"Successfully logged out user with id {refreshToken.UserId}");
      serviceResponse.Data = refreshToken.UserId;
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> Register(RegisterUserDto user)
    {
      try
      {
        // Check email already registered
        if (await _userManager.FindByEmailAsync(user.Email) != null)
        {
          return new ServiceResponse<Guid>()
          {
            Messages = new List<string>() { "Email already in use." },
            Status = HttpStatusCode.BadRequest,
            Success = false
          };
        }

        // Await registration and check for success
        User newUser = new()
        {
          Email = user.Email,
          UserName = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName
        };
        IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
        if (!result.Succeeded)  // Registration failed
        {
          return new ServiceResponse<Guid>()
          {
            Messages = result.Errors.Select(x => x.Description).ToList(),
            Status = HttpStatusCode.BadRequest,
            Success = false
          };
        }
        else  // Add roles/claims and send confirmation email
        {
          User registeredUser = await _userManager.FindByEmailAsync(newUser.Email); // Retrieve the created user to get their Id

          // Add "Participant" role to registered user
          IdentityResult addParticipantRole = await _userManager.AddToRoleAsync(registeredUser, "Participant");

          // Add "Role" claim for registered user
          IdentityResult addParticipantRoleClaim = await _userManager.AddClaimAsync(registeredUser, new Claim(ClaimTypes.Role, "Participant"));
          if (!addParticipantRoleClaim.Succeeded)
          {
            _logger.LogError($"FAILURE: Add claim to 'Participant' role for User '{newUser.Email}'.");
          }
          _logger.LogInformation($"User \"{newUser.Email}\" created a new account.");

          // Generate a confirmation token to securely confirm an email address for a successfully registered user
          string code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
          code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

          // Send callback for route in this controller to ConfirmEmail
          //string callbackUrl = Url.Action("ConfirmEmail", "Authentication", new { userId = createdUser.Id.ToString(), code = code });
          var urlBuilder = new UriBuilder()
          {
            Scheme = "https",
            Host = "psappservice.azurewebsites.net",
            Path = "/api/Authentication/ConfirmEmail",
            Query = $"?userId={registeredUser.Id}&code={code}"
          };

          await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email",
              $"Please confirm your account by <a href='{urlBuilder}'>clicking here</a>.");

          if (_userManager.Options.SignIn.RequireConfirmedAccount)
          {
            return new ServiceResponse<Guid>()
            {
              Messages = new List<string>() { $"Email sent to '{newUser.Email}' with a coded confirmation link (check spam folder)." }
            };
          }
          else
            return new ServiceResponse<Guid>()
            {
              Messages = new List<string>() { $"Email address '{newUser.Email}' automatically confirmed." }
            };
        }
      }
      catch (Exception ex)
      {
        return new ServiceResponse<Guid>()
        {
          Messages = new List<string>() { ex.Message },
          Status = HttpStatusCode.InternalServerError,
          Success = false
        };
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<RefreshTokenDto>> ConfirmEmail(string userId, string code)
    {
      try
      {
        if (userId == null || code == null)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Messages = new List<string>() { "Invalid email confirmation request." },
            Success = false,
            Status = HttpStatusCode.BadRequest
          };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Messages = new List<string>() { $"Unable to load user with ID '{userId}'." },
            Success = false,
            Status = HttpStatusCode.NotFound
          };
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        var response = await GenerateJwt(user);
        response.Messages.Add(result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.");
        return response;
      }
      catch (Exception ex)
      {
        return new ServiceResponse<RefreshTokenDto>()
        {
          Messages = new List<string>() { ex.Message },
          Status = HttpStatusCode.InternalServerError,
          Success = false
        };
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<RefreshTokenDto>> RefreshToken(AuthenticationTokensDto tokens)
    {
      try
      {
        ServiceResponse<RefreshTokenDto> result = await ValidateAndGenerateAuthenticationTokens(tokens);
        if (result == null)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Messages = new List<string>() { "Invalid tokens." },
            Success = false,
            Status = HttpStatusCode.BadRequest
          };
        }
        return result;
      }
      catch (Exception ex)
      {
        return new ServiceResponse<RefreshTokenDto>()
        {
          Messages = new List<string>() { ex.Message },
          Status = HttpStatusCode.InternalServerError,
          Success = false
        };
      }
    }

    /// <summary>
    /// Use a configuration private key to generate a JWT token with a SecurityTokenDescriptor.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<RefreshTokenDto>> GenerateJwt(User user)
    {
      try
      {
        JwtSecurityTokenHandler jwtTokenHandler = new();
        byte[] key = Encoding.UTF8.GetBytes(_jwtConfig.PrivateKey);

        // Establish claims for JWTs.
        List<Claim> claims = new()
        {
          new Claim("UserId", user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.Email, user.Email),             // Email claim of user generating Jwt
          new Claim(JwtRegisteredClaimNames.Sub, user.FirstName),           // Sub claim identifies principal Subject of the JWT
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Enables JWT refresh token functionality
        };

        // Get roles from added user to append as claims - without this none of the JWTs
        // will have a role encoded with each token (by claim) and roles based authorization will fail.
        var roleClaims = await _userManager.GetRolesAsync(user);
        foreach (var role in roleClaims) claims.Add(new Claim(ClaimTypes.Role, role));

        // SecurityTokenDescriptor works to register claims to a JwtPayload (e.g. Subject -> sub, Expires -> exp)
        SecurityTokenDescriptor tokenDescriptor = new()
        {
          Subject = new ClaimsIdentity(claims),
          Expires = DateTime.UtcNow.AddMinutes(10), // use ~5-10 mins in production
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
          ExpiryDate = DateTimeOffset.UtcNow.AddDays(7),  // Should be longer than JWT expiry, unless requiring a new token every time
          Token = RandomString(35) + Guid.NewGuid()
        };

        // Save refreshToken to database
        EntityEntry<RefreshToken> addedRefreshToken = await _db.RefreshTokens.AddAsync(refreshToken);
        await _db.SaveChangesAsync();
        RefreshTokenDto tokenDto = _mapper.Map<RefreshTokenDto>(addedRefreshToken.Entity);
        tokenDto.Jwt = jwtTokenHandler.WriteToken(token);
        tokenDto.RefreshJwt = refreshToken.Token;
        tokenDto.User.Roles = await _userManager.GetRolesAsync(user);

        //return jwtTokenHandler.WriteToken(token);
        return new ServiceResponse<RefreshTokenDto>()
        {
          Data = tokenDto,
          Status = HttpStatusCode.Created,
          Messages = new List<string>() { "Successfully generated new tokens." }
        };
      }
      catch (Exception ex)
      {
        return new ServiceResponse<RefreshTokenDto>()
        {
          Messages = new List<string>() { ex.Message },
          Status = HttpStatusCode.InternalServerError,
          Success = false
        };
      }
    }

    /// <summary>
    /// Helper for validating tokens that are being generated during token refresh.
    /// </summary>
    /// <param name="token">Token from request body being refreshed.</param>
    /// <returns>Jwt with Token and RefreshToken properties</returns>
    public async Task<ServiceResponse<RefreshTokenDto>> ValidateAndGenerateAuthenticationTokens(AuthenticationTokensDto token)
    {
      JwtSecurityTokenHandler jwtTokenHandler = new();  // Handles verification of a Jwt
      try
      {
        // 1. Validate JWT token format to pass on for encryption algorithm validation
        ClaimsPrincipal tokenCandidate = jwtTokenHandler.ValidateToken(token.Jwt, _tokenValidationParams, out SecurityToken validatedToken);

        // 2. Validate encryption algorithm
        if (validatedToken is JwtSecurityToken jwtSecurityToken)
        {
          if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
          {
            return new ServiceResponse<RefreshTokenDto>()
            {
              Success = false,
              Messages = new List<string>() { "Unauthorized." },  // Do not mention algorithm used.
              Status = HttpStatusCode.Unauthorized
            };
          }
        }

        // Attempt to find stored RefreshToken entry in database with Token matching token.RefreshToken 
        RefreshToken storedToken = await _db.RefreshTokens
          .Include(rt => rt.User)
          .FirstOrDefaultAsync(rt => rt.Token == token.RefreshJwt);


        // 3. Validate that stored RefreshToken exists
        if (storedToken == null)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Success = false,
            Messages = new List<string>() { "Token does not exist." },
            Status = HttpStatusCode.Gone
          };
        }

        // 4. Validate that token claim Id from ClaimsPrincipal object (tokenCandidate) returned from 1st validation
        string jti = tokenCandidate.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        if (storedToken.JwtId.ToString() != jti)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Success = false,
            Messages = new List<string>() { "Token does not match." },
            Status = HttpStatusCode.Conflict
          };
        }

        // 5. Validate that stored RefreshToken has not been revoked
        if (storedToken.IsRevoked)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Success = false,
            Messages = new List<string>() { "Token has been revoked." },
            Status = HttpStatusCode.Forbidden
          };
        }

        // 6. Validate that stored RefreshToken has not been used
        if (storedToken.IsUsed)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Success = false,
            Messages = new List<string>() { "Token has been used." },
            Status = HttpStatusCode.Forbidden
          };
        }

        // 7. Validate expiry date is after now (no need to refresh)
        long utcExpiryDate = long.Parse(tokenCandidate.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
        DateTimeOffset expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
        if (expiryDate > DateTime.UtcNow)
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Success = false,
            Messages = new List<string>() { "Token has not expired." },
            Status = HttpStatusCode.OK,
            Data = _mapper.Map<RefreshTokenDto>(storedToken)
          };
        }

        storedToken.IsUsed = true;              // Set stored RefreshToken as used
        _db.RefreshTokens.Update(storedToken);  // Update stored RefreshToken
        await _db.SaveChangesAsync();           // Save changes

        // Generate and return new Jwt for the UserId associated to stored RefreshToken
        return await GenerateJwt(await _userManager.FindByIdAsync(storedToken.UserId.ToString()));
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Success = false,
            Messages = new List<string>() { "Token has expired please login again." },
            Status = HttpStatusCode.Gone
          };
        }
        else
        {
          return new ServiceResponse<RefreshTokenDto>()
          {
            Success = false,
            Messages = new List<string>() { "Something went wrong." },
            Status = HttpStatusCode.BadRequest
          };
        }
      }
    }

    /// <summary>
    /// Access current HTTP context to get authenticated user and retrieve their Id via custom UserId claim.
    /// </summary>
    /// <returns></returns>
    public Guid GetUserId()
    {
      var claims = _httpContextAccessor.HttpContext.User.Identities.First().Claims.ToList();
      var userIdClaim = claims.FirstOrDefault(c => c.Type == "UserId").Value;
      return Guid.Parse(userIdClaim);
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
