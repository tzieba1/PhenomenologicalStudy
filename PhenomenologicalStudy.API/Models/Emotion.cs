using PhenomenologicalStudy.API.Models.ManyToMany;
using ReflectionAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Emotion
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    public EmotionType Type { get; set; }
    public int Intensity { get; set; }
    public ReflectionChild ReflectionChild { get; set; }  // ReflectionChild 1...* Emotion
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
  }
}
