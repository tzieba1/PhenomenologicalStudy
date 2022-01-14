using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Badge
{
  public class GetBadgeDto
  {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public BadgeType Type { get; set; }
    public int Value { get; set; }
    public string Message { get; set; }
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
  }
}
