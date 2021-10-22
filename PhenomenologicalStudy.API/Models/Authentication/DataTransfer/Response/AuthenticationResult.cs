using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.Authentication.DataTransfer.Response
{
  /// <summary>
  /// Base class responsible for data transfer objects.
  /// </summary>
  public class AuthenticationResult
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; }
  }
}
