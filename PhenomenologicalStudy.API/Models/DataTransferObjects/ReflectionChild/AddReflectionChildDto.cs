using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild
{
  public class AddReflectionChildDto
  {
    public Guid ReflectionId { get; set; }
    public Guid ChildId { get; set; }
  }
}
