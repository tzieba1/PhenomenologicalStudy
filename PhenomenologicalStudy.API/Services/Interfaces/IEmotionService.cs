using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IEmotionService
  {
    /// <summary>
    /// Represents EmotionsController endpoint 'DELETE: /api/Emotions/{id}' defined in EmotionService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetEmotionDto>> DeleteEmotionById(Guid id);

    /// <summary>
    /// Represents EmotionsController endpoint 'POST: /api/Emotions' defined in EmotionService.
    /// </summary>
    /// <param name="emotion"></param>
    /// <param name="reflectionChildId"></param>
    /// <returns></returns>
    Task<ServiceResponse<Guid>> PostEmotion(AddEmotionDto emotion, Guid? reflectionChildId);

    /// <summary>
    /// Represents EmotionsController endpoint 'PUT: /api/Emotions?reflectionChildId={Guid?}' defined in EmotionService.
    /// </summary>
    /// <param name="emotion"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetEmotionDto>> PutEmotion(UpdateEmotionDto emotion);

    /// <summary>
    /// Represents EmotionsController endpoint 'GET: /api/Emotions/{id}' defined in EmotionService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetEmotionDto>> GetEmotionById(Guid id);

    /// <summary>
    /// Represents EmotionsController endpoint 'DELETE: /api/Emotions' defined in EmotionService.
    /// </summary>
    /// <returns></returns>
    Task<ServiceResponse<List<GetEmotionDto>>> GetEmotions();
  }
}
