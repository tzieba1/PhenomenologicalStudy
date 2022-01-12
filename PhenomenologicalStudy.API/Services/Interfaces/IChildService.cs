using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IChildService
  {
    Task<ServiceResponse<GetChildDto>> DeleteChildById(Guid id);
    Task<ServiceResponse<Guid>> PostChild(AddChildDto Child, Guid? userId);
    Task<ServiceResponse<GetChildDto>> PutChild(UpdateChildDto Child);
    Task<ServiceResponse<GetChildDto>> GetChildById(Guid id);
    Task<ServiceResponse<List<GetChildDto>>> GetChildren();
  }
}