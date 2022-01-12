using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Capture;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Comment;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection;
using PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IReflectionService
  {
    Task<ServiceResponse<List<GetReflectionDto>>> GetAllReflections();
    Task<ServiceResponse<List<GetUserReflectionDto>>> GetUserReflections();
    Task<ServiceResponse<List<GetCaptureDto>>> GetReflectionCaptures();
    Task<ServiceResponse<GetCaptureDto>> GetReflectionCaptureById(Guid id);
    Task<ServiceResponse<GetUserReflectionDto>> GetReflectionById(Guid id);
    Task<ServiceResponse<Guid>> PostReflection(AddReflectionStringDataDto reflection);
    Task<ServiceResponse<Guid>> DeleteReflection(Guid id);
    Task<ServiceResponse<GetCommentDto>> UpdateReflectionComment(UpdateReflectionCommentDto reflectionComment);
    Task<ServiceResponse<Guid>> PostReflectionChildEmotion(AddReflectionChildEmotionDto reflectionChildEmotion);
    Task<ServiceResponse<Guid>> PostReflectionChild(AddReflectionChildDto reflectionChild);
    Task<ServiceResponse<Guid>> DeleteReflectionChild(Guid reflectionId, Guid ChildId);
  }
}
