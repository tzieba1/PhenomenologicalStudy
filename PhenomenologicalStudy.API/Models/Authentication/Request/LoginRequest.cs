using System.ComponentModel.DataAnnotations;

namespace PhenomenologicalStudy.API.Models.Authentication.Request
{
  public class LoginRequest
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}