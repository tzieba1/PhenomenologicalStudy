using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Authentication.Response
{
  /// <summary>
  /// Base class responsible for data transfer objects.
  /// </summary>
  public class AuthenticationResult : Jwt
  {
    public bool Success { get; set; }
    public List<string> Errors { get; set; }

    public List<string> StatusMessages { get; set; }

    public AuthenticationResult() { }

    public AuthenticationResult(Jwt jwtToken, bool success)
    {
      this.Success = success;
      this.Token = jwtToken.Token;
      this.RefreshToken = jwtToken.RefreshToken;
    }
  }
}
