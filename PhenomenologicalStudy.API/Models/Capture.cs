using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhenomenologicalStudy.API.Models
{
  public class Capture
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    public Reflection Reflection { get; set; }  // Reflection 1...1 Capture
    public Guid ReflectionId { get; set; }  // Capture depends on Reflection (delete Reflection -> delete Capture)
    public byte[] Data { get; set; }
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
  }
}
