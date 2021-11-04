using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Child
  {
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Required]
    public DateTimeOffset DateOfBirth { get; set; }

    [Required]
    public char Gender { get; set; }

    public User Users { get; set; }

    public virtual ICollection<Emotion> Emotions { get; set; }
  }
}
