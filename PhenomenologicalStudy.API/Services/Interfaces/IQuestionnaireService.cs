using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Questionnaire;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IQuestionnaireService
  {
    /// <summary>
    /// Represents QuestionnairesController endpoint 'DELETE: /api/Questionnaires/{id}' defined in QuestionnaireService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetQuestionnaireDto>> DeleteQuestionnaireById(Guid id);

    /// <summary>
    /// Represents QuestionnairesController endpoint 'POST: /api/Questionnaires?userId={Guid?}' defined in QuestionnaireService.
    /// </summary>
    /// <param name="questionnaire"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ServiceResponse<Guid>> PostQuestionnaire(AddQuestionnaireDto questionnaire, Guid? userId);

    /// <summary>
    /// Represents QuestionnairesController endpoint 'PUT: /api/Questionnaires' defined in QuestionnaireService.
    /// </summary>
    /// <param name="questionnaire"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetQuestionnaireDto>> PutQuestionnaire(UpdateQuestionnaireDto questionnaire);


    /// <summary>
    /// Represents QuestionnairesController endpoint 'GET: /api/Questionnaires/{id}' defined in QuestionnaireService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetQuestionnaireDto>> GetQuestionnaireById(Guid id);

    /// <summary>
    /// Represents QuestionnairesController endpoint 'GET: /api/Questionnaires' defined in QuestionnaireService.
    /// </summary>
    /// <returns></returns>
    Task<ServiceResponse<List<GetQuestionnaireDto>>> GetQuestionnaires();
  }
}
