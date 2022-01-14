using System;
using System.ComponentModel.DataAnnotations;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.User
{
  public class AddUserDto
  {
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public DateTimeOffset? BirthDate { get; set; }
  }
}
