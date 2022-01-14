using System;
using System.ComponentModel.DataAnnotations;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.User
{
  public class UpdateUserDto
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [DataType(DataType.Date)]
    public DateTimeOffset? BirthDate { get; set; }
    public string Role { get; set; }
  }
}
