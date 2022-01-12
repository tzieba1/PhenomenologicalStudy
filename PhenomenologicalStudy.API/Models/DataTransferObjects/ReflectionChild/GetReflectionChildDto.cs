using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild
{
  public class GetReflectionChildDto
  {
    public Guid Id { get; set; }
    public GetChildDto Child { get; set; }
    public ICollection<GetEmotionDto> Emotions { get; set; }
  }
}
