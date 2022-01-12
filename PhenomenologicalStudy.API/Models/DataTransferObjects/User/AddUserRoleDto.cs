using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.User
{
  public class AddUserRoleDto
  {
    public Guid UserId { get; set; }
    public string Name { get; set; }
  }
}
