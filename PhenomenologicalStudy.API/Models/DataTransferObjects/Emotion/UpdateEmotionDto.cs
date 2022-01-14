using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion
{
  public class UpdateEmotionDto
  {
    public Guid Id { get; set; }
    public EmotionType Type { get; set; }
    public int Intensity { get; set; }
  }
}
