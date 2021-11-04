﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Emotion
  {
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }
    
    public int? Intensity { get; set; }
  }
}
