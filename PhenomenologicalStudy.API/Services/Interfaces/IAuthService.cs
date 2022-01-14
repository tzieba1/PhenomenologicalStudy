using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using System;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IAuthService
  {
    /// <summary>
    /// Represents AuthenticationController endpoint 'DELETE: /api/Authentication/Register' defined in AuthenticationService.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<ServiceResponse<Guid>> Register(RegisterUserDto user);

    /// <summary>
    /// Represents AuthenticationController endpoint 'POST: /api/Authentication/Login' defined in AuthenticationController.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<ServiceResponse<RefreshTokenDto>> Login(LoginUserDto user);

    /// <summary>
    /// Represents AuthenticationController endpoint 'POST: /api/Authentication/Logout' defined in AuthenticationController.
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    Task<ServiceResponse<Guid>> Logout(AuthenticationTokensDto tokens);

    /// <summary>
    /// Represents AuthenticationController endpoint 'POST: /api/Authentication/ConfirmEmail?userId={string}&code={string}' defined in AuthenticationController.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<ServiceResponse<RefreshTokenDto>> ConfirmEmail(string userId, string code);

    /// <summary>
    /// Represents AuthenticationController endpoint 'POST: /api/Authentication/RefreshToken' defined in AuthenticationController.
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    Task<ServiceResponse<RefreshTokenDto>> RefreshToken(AuthenticationTokensDto tokens);

    /// <summary>
    /// Helper used with RefreshToken method to take steps and validate a JWT and its refresh token before generating new authentication tokens.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<ServiceResponse<RefreshTokenDto>> ValidateAndGenerateAuthenticationTokens(AuthenticationTokensDto token);

    /// <summary>
    /// Helper used with ConfirmEmail, Login, and RefreshToken to generate a new JWT with claims for a confirmed user and a refresh token.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<ServiceResponse<RefreshTokenDto>> GenerateJwt(User user);

    /// <summary>
    /// Helper used to retrieve a bearer's 'userId' claim from the JWT provided within the Authorization header.
    /// </summary>
    /// <returns></returns>
    Guid GetUserId();
  }
}
