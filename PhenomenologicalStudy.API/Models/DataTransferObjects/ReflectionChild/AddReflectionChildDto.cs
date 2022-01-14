using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild
{
  public class AddReflectionChildDto
  {
    public Guid ReflectionId { get; set; }
    public Guid ChildId { get; set; }
  }
}
