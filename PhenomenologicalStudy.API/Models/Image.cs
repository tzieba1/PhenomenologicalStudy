using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PhenomenologicalStudy.API.Models
{
  public class Image
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public byte[] Data { get; set; }

    public ICollection<Reflection> Reflections { get; set; }
  }
}
