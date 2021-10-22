using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Reflection
  {
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ChildId { get; set; }

    [Required]
    public Guid ImageId { get; set; }
        
    [Required]
    public Guid CommentId { get; set; }
    
    [Required]
    public DateTimeOffset Created { get; set; }
    
    [Required]
    public DateTimeOffset Updated { get; set; }
  }
}
