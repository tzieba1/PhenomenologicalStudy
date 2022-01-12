using PhenomenologicalStudy.API.Models.ManyToMany;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PhenomenologicalStudy.API.Models
{
  public class Reflection
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    public User User { get; set; }  // User 1...* Reflection
    public Capture Capture { get; set; }  // Capture 1...1 Reflection
    public Comment Comment { get; set; }  // Comment 1...1 Reflection
    public List<ReflectionChild> Children { get; set; } // ReflectionChild *...1 Reflection
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
  }
}
