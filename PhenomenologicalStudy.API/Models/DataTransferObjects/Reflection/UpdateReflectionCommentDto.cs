using PhenomenologicalStudy.API.Models.DataTransferObjects.Comment;
using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection
{
  public class UpdateReflectionCommentDto
  {
    public Guid ReflectionId { get; set; }
    public UpdateCommentDto Comment { get; set; }
  }
}
