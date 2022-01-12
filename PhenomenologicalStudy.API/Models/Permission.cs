using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Permission
  { 
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

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
