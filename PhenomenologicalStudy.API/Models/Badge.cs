using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Badge
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    public BadgeType Type { get; set; }
    public int Value { get; set; }
    public string Message { get; set; }
    public User User { get; set; }  // User 1...* Badge (remember to add IEnumerable<Badge> to User model before migrating)
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
  }
}
