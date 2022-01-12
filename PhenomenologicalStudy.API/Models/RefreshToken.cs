using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class RefreshToken
  {
    [Key]
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid JwtId { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }
    public User User { get; set; }  // User 1...* RefreshToken
    public Guid UserId { get; set; }  // RefreshToken depends on User (delete User -> delete RefreshToken)
  }
}
