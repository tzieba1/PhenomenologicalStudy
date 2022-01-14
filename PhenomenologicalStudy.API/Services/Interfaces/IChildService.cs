using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IChildService
  {
    /// <summary>
    /// Represents ChildrenController endpoint 'DELETE: /api/Children/{id}' defined in ChildService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetChildDto>> DeleteChildById(Guid id);

    /// <summary>
    /// Represents ChildrenController endpoint 'POST: /api/Children?userId={Guid?}' defined in ChildService.
    /// </summary>
    /// <param name="Child"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ServiceResponse<Guid>> PostChild(AddChildDto Child, Guid? userId);

    /// <summary>
    /// Represents ChildrenController endpoint 'PUT: /api/Children' defined in ChildService.
    /// </summary>
    /// <param name="Child"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetChildDto>> PutChild(UpdateChildDto Child);

    /// <summary>
    /// Represents ChildrenController endpoint 'GET: /api/Children/{id}' defined in ChildService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetChildDto>> GetChildById(Guid id);

    /// <summary>
    /// Represents ChildrenController endpoint 'GET: /api/Children' defined in ChildService.
    /// </summary>
    /// <returns></returns>
    Task<ServiceResponse<List<GetChildDto>>> GetChildren();
  }
}