using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.ManyToMany
{
  public class ReflectionChild
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    public Guid ChildId { get; set; } // ReflectionChild depends on Child (delete Child -> delete ReflectionChild)
    public Child Child { get; set; }  // ReflectionChild 1...1 Child
    public Reflection Reflection { get; set; } // Reflection 1...* ReflectionChild
    public ICollection<Emotion> Emotions { get; set; }  // Emotion *...1 ReflectionChild
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
  }
}
