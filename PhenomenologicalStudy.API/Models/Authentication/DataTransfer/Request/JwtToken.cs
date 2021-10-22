using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.Authentication.DataTransfer.Request
{
  public class JwtToken
  {
    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }
  }
}
