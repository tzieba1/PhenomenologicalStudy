using PhenomenologicalStudy.API.Models.ManyToMany;
using System;
using System.ComponentModel.DataAnnotations;

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
