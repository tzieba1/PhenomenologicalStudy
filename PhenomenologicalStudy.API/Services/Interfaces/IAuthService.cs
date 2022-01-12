using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IAuthService
  {
    Task<ServiceResponse<Guid>> Register(RegisterUserDto user);
    Task<ServiceResponse<AuthenticationTokens>> Login(LoginUserDto user);
    Task<ServiceResponse<AuthenticationTokens>> Logout(AuthenticationTokens tokens);
    Task<ServiceResponse<AuthenticationTokens>> ConfirmEmail(string userId, string code);
    Task<ServiceResponse<AuthenticationTokens>> RefreshToken(AuthenticationTokens tokens);
    Task<ServiceResponse<AuthenticationTokens>> ValidateAndGenerateAuthenticationTokens(AuthenticationTokens tokens);
    Task<ServiceResponse<AuthenticationTokens>> GenerateJwt(User user);
    Guid GetUserId();
  }
}
