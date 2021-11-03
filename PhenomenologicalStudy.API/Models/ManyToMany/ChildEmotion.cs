using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PhenomenologicalStudy.API.Models
{
  public class ChildEmotion
  {
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ChildId { get; set; }

    [Required]
    public Guid EmotionId { get; set; }

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

    // Foreign keys establish one-to-many relationship 
    [ForeignKey(nameof(ChildId))]
    public Child Child { get; set; }

    [ForeignKey(nameof(EmotionId))]
    public Emotion Emotion { get; set; }

    public ICollection<Reflection> Reflections { get; set; }
  }
}
