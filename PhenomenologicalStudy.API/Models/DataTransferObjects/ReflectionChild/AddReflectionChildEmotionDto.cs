using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild
{
  public class AddReflectionChildEmotionDto
  {
    public Guid ChildId { get; set; }
    public Guid ReflectionId { get; set; }
    public AddEmotionDto Emotion { get; set; }
  }
}
