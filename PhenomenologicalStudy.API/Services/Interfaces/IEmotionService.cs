using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IEmotionService
  {
    Task<ServiceResponse<GetEmotionDto>> DeleteEmotionById(Guid id);
    Task<ServiceResponse<Guid>> PostEmotion(AddEmotionDto emotion, Guid? reflectionChildId);
    Task<ServiceResponse<GetEmotionDto>> PutEmotion(UpdateEmotionDto emotion);
    Task<ServiceResponse<GetEmotionDto>> GetEmotionById(Guid id);
    Task<ServiceResponse<List<GetEmotionDto>>> GetEmotions();
  }
}
