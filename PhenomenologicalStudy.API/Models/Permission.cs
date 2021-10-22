using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Permission
  {
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public bool Camera { get; set; }

    [Required]
    public bool PictureLibrary { get; set; }

    [Required]
    public bool FileSystem { get; set; }

    [Required]
    public bool Microphone { get; set; }

    [Required]
    public bool Badges { get; set; }
  }
}
