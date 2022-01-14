using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection
{
  public class AddReflectionChildEmotionDto
  {
    public Guid ChildId { get; set; }
    public AddEmotionDto Emotion { get; set; }
  }
}
