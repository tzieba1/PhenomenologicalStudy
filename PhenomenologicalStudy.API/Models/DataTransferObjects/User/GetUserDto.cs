using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
    public string Role { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
  }
}
