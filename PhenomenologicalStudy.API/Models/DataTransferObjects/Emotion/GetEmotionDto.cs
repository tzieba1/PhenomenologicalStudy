using ReflectionAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion
{
  public class GetEmotionDto
  {
    public Guid Id { get; set; }
    public EmotionType Type { get; set; }
    public int Intensity { get; set; }
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
  }
}
