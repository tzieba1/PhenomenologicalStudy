using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Badge
{
  public class UpdateBadgeDto
  {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Value { get; set; }
    public string Message { get; set; }
  }
}
