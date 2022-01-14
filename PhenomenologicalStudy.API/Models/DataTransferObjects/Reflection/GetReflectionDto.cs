using PhenomenologicalStudy.API.Models.DataTransferObjects.Capture;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Comment;
using PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using System;
using System.Collections.Generic;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection
{
  public class GetReflectionDto
  {
    public Guid Id { get; set; }
    public GetUserDto User { get; set; }
    public GetCommentDto Comment { get; set; }
    public GetCaptureDto Capture { get; set; }
    public ICollection<GetReflectionChildDto> Children { get; set; }
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
  }
}
