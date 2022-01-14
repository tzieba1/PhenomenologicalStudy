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
    /// <summary>
    /// Represents ReflectionsController endpoint 'GET: /api/Reflections' defined in ReflectionService.
    /// </summary>
    /// <returns></returns>
    Task<ServiceResponse<List<GetReflectionDto>>> GetReflections();

    /// <summary>
    /// Represents ReflectionsController endpoint 'POST: /api/Reflections/Captures' defined in ReflectionService.
    /// </summary>
    /// <returns></returns>
    Task<ServiceResponse<List<GetCaptureDto>>> GetReflectionCaptures();

    /// <summary>
    /// Represents ReflectionsController endpoint 'GET: /api/Reflections/{rid}/Capture' defined in ReflectionService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetCaptureDto>> GetReflectionCaptureById(Guid id);

    /// <summary>
    /// Represents ReflectionsController endpoint 'GET: /api/Reflections/{id}' defined in ReflectionService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetReflectionDto>> GetReflectionById(Guid id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflection"></param>
    /// <returns></returns>
    Task<ServiceResponse<Guid>> PostReflection(AddReflectionStringDataDto reflection);
    Task<ServiceResponse<Guid>> DeleteReflection(Guid id);
    Task<ServiceResponse<GetCommentDto>> UpdateReflectionComment(UpdateReflectionCommentDto reflectionComment);
    Task<ServiceResponse<Guid>> PostReflectionChildEmotion(Guid reflectionId, AddReflectionChildEmotionDto childEmotion);
    Task<ServiceResponse<Guid>> DeleteReflectionChildEmotion(Guid reflectionId, RemoveReflectionChildEmotionDto childEmotion);
    Task<ServiceResponse<Guid>> PostReflectionChild(AddReflectionChildDto reflectionChild);
    Task<ServiceResponse<Guid>> DeleteReflectionChild(Guid reflectionId, Guid ChildId);
  }
}
