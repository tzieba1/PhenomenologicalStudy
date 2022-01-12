using PhenomenologicalStudy.API.Models.DataTransferObjects.Comment;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection
{
  public class UpdateReflectionCommentDto
  {
    public Guid ReflectionId { get; set; }
    public UpdateCommentDto Comment { get; set; }
  }
}
