using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
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
  public class EmotionsController : ControllerBase
  {
    private readonly IEmotionService _emotionService;

    public EmotionsController(IEmotionService emotionService)
    {
      _emotionService = emotionService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emotion"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetEmotionDto>>> GetEmotionById(Guid id)
    {
      ServiceResponse<GetEmotionDto> response = await _emotionService.GetEmotionById(id);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => BadRequest(new ServiceResponse<GetEmotionDto>()
        {
          Messages = new List<string>() { "Invalid payload." },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emotion"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetEmotionDto>>>> GetEmotions()
    {
      ServiceResponse<List<GetEmotionDto>> response = await _emotionService.GetEmotions();
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => BadRequest(new ServiceResponse<GetEmotionDto>()
        {
          Messages = new List<string>() { "Invalid payload." },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emotion"></param>
    /// <returns></returns>
    [HttpPost("ReflectionChild")]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostEmotion(AddEmotionDto emotion, Guid? reflectionChildId)
    {
      ServiceResponse<Guid> response = await _emotionService.PostEmotion(emotion, reflectionChildId);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => BadRequest(new ServiceResponse<GetEmotionDto>()
        {
          Messages = new List<string>() { "Invalid payload." },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<ServiceResponse<GetEmotionDto>>> DeleteEmotion(Guid id)
    {
      ServiceResponse<GetEmotionDto> response = await _emotionService.DeleteEmotionById(id);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => BadRequest(new ServiceResponse<GetEmotionDto>()
        {
          Messages = new List<string>() { "Invalid payload." },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emotion"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<ServiceResponse<GetEmotionDto>>> PutEmotion(UpdateEmotionDto emotion)
    {
      ServiceResponse<GetEmotionDto> response = await _emotionService.PutEmotion(emotion);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => BadRequest(new ServiceResponse<GetEmotionDto>()
        {
          Messages = new List<string>() { "Invalid payload." },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }
  }
}