using System.ComponentModel.DataAnnotations;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects
{
  public class AuthenticationTokensDto
  {
    [Required]
    public string Jwt { get; set; }
    [Required]
    public string RefreshJwt { get; set; }
  }
}
