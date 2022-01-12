using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects
{
  public class AuthenticationTokens
  {
    [Required]
    public string Jwt { get; set; }
    [Required]
    public string RefreshJwt { get; set; }
  }
}
