using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects
{
  public class RefreshTokenDto
  {
    public string Jwt { get; set; }
    public string RefreshJwt { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }
    public GetUserDto User { get; set; }
  }
}
