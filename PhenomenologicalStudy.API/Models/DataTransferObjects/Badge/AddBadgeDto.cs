using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Badge
{
  public class AddBadgeDto
  {
    public Guid UserId { get; set; }
    public BadgeType Type { get; set; }
    public int Value { get; set; }
    public string Message { get; set; }
  }
}
