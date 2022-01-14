using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.User
{
  public class AddUserRoleDto
  {
    public Guid UserId { get; set; }
    public string Name { get; set; }
  }
}
