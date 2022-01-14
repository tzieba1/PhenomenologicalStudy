using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Capture;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Comment;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection;
using PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(Roles = "Admin, Participant")]
  [ApiController]
  public class ReflectionsController : ControllerBase
  {
    private readonly IReflectionService _reflectionService;

    public ReflectionsController(IReflectionService reflectionService)
    {
      _reflectionService = reflectionService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<Guid>>> DeleteReflection(Guid id)
    {
      ServiceResponse<Guid> response = await _reflectionService.DeleteReflection(id);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetReflectionDto>>>> GetReflections()
    {
      ServiceResponse<List<GetReflectionDto>> response = await _reflectionService.GetReflections();
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
    /// <returns></returns>
    [HttpGet("Captures")]
    public async Task<ActionResult<ServiceResponse<List<GetCaptureDto>>>> GetReflectionCaptures()
    {
      ServiceResponse<List<GetCaptureDto>> response = await _reflectionService.GetReflectionCaptures();
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflectionChild"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpPost("Children")]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostReflectionChild(AddReflectionChildDto reflectionChild)
    {
      ServiceResponse<Guid> response = await _reflectionService.PostReflectionChild(reflectionChild);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.Conflict => Conflict(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rid"></param>
    /// <param name="cid"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpDelete("{rid}/Children/{cid}")]
    public async Task<ActionResult<ServiceResponse<Guid>>> DeleteReflectionChild(Guid rid, Guid cid)
    {
      ServiceResponse<Guid> response = await _reflectionService.DeleteReflectionChild(rid, cid);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflectionChildEmotion"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpPost("{rid}/ChildEmotions")]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostReflectionChildEmotion(Guid rid, AddReflectionChildEmotionDto childEmotion)
    {
      ServiceResponse<Guid> response = await _reflectionService.PostReflectionChildEmotion(rid, childEmotion);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.Conflict => Conflict(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    [Authorize(Roles = "Participant")]
    [HttpDelete("{rid}/ChildEmotions")]
    public async Task<ActionResult<ServiceResponse<Guid>>> DeleteReflectionChildEmotion(Guid rid, RemoveReflectionChildEmotionDto childEmotion)
    {
      ServiceResponse<Guid> response = await _reflectionService.DeleteReflectionChildEmotion(rid, childEmotion);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("{rid}/Capture")]
    public async Task<ActionResult<ServiceResponse<GetCaptureDto>>> GetReflectionCaptureById(Guid rid)
    {
      ServiceResponse<GetCaptureDto> response = await _reflectionService.GetReflectionCaptureById(rid);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetReflectionDto>>> GetReflectionById(Guid id)
    {
      ServiceResponse<GetReflectionDto> response = await _reflectionService.GetReflectionById(id);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflection"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostReflection(AddReflectionStringDataDto reflection)
    {
      ServiceResponse<Guid> response = await _reflectionService.PostReflection(reflection);
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
    /// <param name="updatedComment"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpPut("Comment")]
    public async Task<ActionResult<ServiceResponse<GetCommentDto>>> UpdateReflectionComment(UpdateReflectionCommentDto updatedComment)
    {
      ServiceResponse<GetCommentDto> response = await _reflectionService.UpdateReflectionComment(updatedComment);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }
  }
}
