using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class ChildEmotion
  {
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ChildId { get; set; }

    [Required]
    public Guid EmotionId { get; set; }
  }
}
