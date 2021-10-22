using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class UserReflection
  {
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid ReflectionId { get; set; }
  }
}
