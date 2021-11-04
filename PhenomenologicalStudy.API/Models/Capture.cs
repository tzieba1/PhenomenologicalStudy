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

    [Required]
    public Guid ReflectionID { get; set; }
    
    [ForeignKey(nameof(ReflectionID))]
    public Reflection Reflection { get; set; }

    [Required]
    public byte[] Data { get; set; }
  }
}
