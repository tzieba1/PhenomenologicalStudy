using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Comment
  {
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public DateTimeOffset CreatedTime { get; set; }

    [Required]
    public DateTimeOffset UpdatedTime { get; set; }
  }
}
