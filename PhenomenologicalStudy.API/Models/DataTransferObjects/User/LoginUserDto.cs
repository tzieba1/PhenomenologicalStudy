using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.User
{
  public class LoginUserDto
  {
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
  }
}
