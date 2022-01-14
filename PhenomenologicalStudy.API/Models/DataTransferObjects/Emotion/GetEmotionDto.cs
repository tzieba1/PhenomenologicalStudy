using System;

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
