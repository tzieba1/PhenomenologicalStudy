using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PhenomenologicalStudy.API.Models
{
  public class Comment
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    public Reflection Reflection { get; set; }  // Comment 1...1 Reflection
    public Guid ReflectionId { get; set; }  // Comment depends on Reflection (delete Reflection -> delete Comment)
    public string Text { get; set; } = "";
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
  }
}
