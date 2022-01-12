using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IUserService
  {
    Task<ServiceResponse<GetUserDto>> DeleteUserById(Guid id);
    Task<ServiceResponse<Guid>> PostUser(AddUserDto User);
    Task<ServiceResponse<GetUserDto>> PutUser(UpdateUserDto User);
    Task<ServiceResponse<GetUserDto>> GetUserById(Guid id);
    Task<ServiceResponse<List<GetUserDto>>> GetUsers();
    Task<ServiceResponse<List<GetUserRoleDto>>> GetUserRoles(Guid? id);
    Task<ServiceResponse<List<GetUserRoleDto>>> PostUserRole(AddUserRoleDto role);
  }
}
