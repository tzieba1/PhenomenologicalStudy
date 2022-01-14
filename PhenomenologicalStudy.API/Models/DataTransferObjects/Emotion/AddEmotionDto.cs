namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion
{
  public class AddEmotionDto
  {
    public EmotionType Type { get; set; }
    public int Intensity { get; set; }
  }
}
