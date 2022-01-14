using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.User
{
  public class GetUserDto
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [DataType(DataType.Date)]
    public DateTimeOffset BirthDate { get; set; }
    public byte[] ProfilePicture { get; set; }
    public IList<string> Roles { get; set; }
  }
}
