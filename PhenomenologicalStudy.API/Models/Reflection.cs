﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PhenomenologicalStudy.API.Models
{
  public class Reflection
  {
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Guid ChildEmotionId { get; set; }

    [Required]
    public Guid ImageId { get; set; }
        
    [Required]
    public Guid CommentId { get; set; }

    [NotMapped]
    [JsonIgnore]
    [XmlElement("CreationTime")]
    public string CreationTimeOnForXml // format: 2011-11-11T15:05:46.4733406+01:00
    {
      get { return CreationTime.ToString("o"); } // o = yyyy-MM-ddTHH:mm:ss.fffffffzzz
      set { CreationTime = DateTimeOffset.Parse(value); }
    }

    [XmlIgnore]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset CreationTime { get; set; }

    [NotMapped]
    [JsonIgnore]
    [XmlElement("UpdatedTime")]
    public string UpdatedTimeForXml // format: 2011-11-11T15:05:46.4733406+01:00
    {
      get { return UpdatedTime.ToString("o"); } // o = yyyy-MM-ddTHH:mm:ss.fffffffzzz
      set { UpdatedTime = DateTimeOffset.Parse(value); }
    }

    [XmlIgnore]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset UpdatedTime { get; set; }

    // --- FOREIGN KEYS --- //
    [ForeignKey(nameof(ChildEmotionId))]
    public ChildEmotion ChildEmotion { get; set; }

    [ForeignKey(nameof(ImageId))]
    public Image Image { get; set; }

    [ForeignKey(nameof(CommentId))]
    public Comment Comment { get; set; }

  }
}
