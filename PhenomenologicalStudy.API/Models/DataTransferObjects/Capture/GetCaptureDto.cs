using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Capture
{
  public class GetCaptureDto
  {
    public Guid Id { get; set; }
    public Guid ReflectionId { get; set; }
    public byte[] Data { get; set; }
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset UpdatedTime { get; set; }
  }
}
