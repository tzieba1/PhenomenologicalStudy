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

    [Required]
    public Guid ReflectionID { get; set; }

    [ForeignKey(nameof(ReflectionID))]
    public Reflection Reflection { get; set; }

    [Required]
    public string Text { get; set; }

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
  }
}
