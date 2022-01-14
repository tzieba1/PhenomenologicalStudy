using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Comment
{
  public class GetCommentDto
  {
    public string Text { get; set; }
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
  }
}
