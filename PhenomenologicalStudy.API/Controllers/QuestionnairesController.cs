using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Questionnaire;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(Roles = "Admin, Participant")]
  public class QuestionnairesController : ControllerBase
  {
    private readonly IQuestionnaireService _questionnaireService;

    public QuestionnairesController(IQuestionnaireService questionnaireService)
    {
      _questionnaireService = questionnaireService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="questionnaire"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetQuestionnaireDto>>> GetQuestionnaireById(Guid id)
    {
      ServiceResponse<GetQuestionnaireDto> response = await _questionnaireService.GetQuestionnaireById(id);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="questionnaire"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetQuestionnaireDto>>>> GetQuestionnaires()
    {
      ServiceResponse<List<GetQuestionnaireDto>> response = await _questionnaireService.GetQuestionnaires();
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="questionnaire"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostQuestionnaire(AddQuestionnaireDto questionnaire, Guid? userId)
    {
      ServiceResponse<Guid> response = await _questionnaireService.PostQuestionnaire(questionnaire, userId);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<ServiceResponse<GetQuestionnaireDto>>> DeleteQuestionnaire(Guid id)
    {
      ServiceResponse<GetQuestionnaireDto> response = await _questionnaireService.DeleteQuestionnaireById(id);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="questionnaire"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<ServiceResponse<GetQuestionnaireDto>>> PutQuestionnaire(UpdateQuestionnaireDto questionnaire)
    {
      ServiceResponse<GetQuestionnaireDto> response = await _questionnaireService.PutQuestionnaire(questionnaire);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }
  }
}
