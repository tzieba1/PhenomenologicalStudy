using System;
namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection
{
  public class RemoveReflectionChildEmotionDto
  {
    public Guid ChildId { get; set; }
    public Guid EmotionId { get; set; }

  }
}
