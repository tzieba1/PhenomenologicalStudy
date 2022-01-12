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
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize]
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
      return Ok(await _reflectionService.DeleteReflection(id));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetReflectionDto>>>> GetUserReflections()
    {
      return Ok(await _reflectionService.GetUserReflections());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Participant, Admin")]
    [HttpGet("Captures")]
    public async Task<ActionResult<ServiceResponse<List<GetCaptureDto>>>> GetReflectionCaptures()
    {
      return Ok(await _reflectionService.GetReflectionCaptures());
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
      return Ok(await _reflectionService.PostReflectionChild(reflectionChild));
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
      return Ok(await _reflectionService.DeleteReflectionChild(rid, cid));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflectionChildEmotion"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpPost("Children/Emotions")]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostReflectionChildEmotion(AddReflectionChildEmotionDto reflectionChildEmotion)
    {
      return Ok(await _reflectionService.PostReflectionChildEmotion(reflectionChildEmotion));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Participant, Admin")]
    [HttpGet("{id}/Capture")]
    public async Task<ActionResult<ServiceResponse<GetCaptureDto>>> GetReflectionCaptureById(Guid id)
    {
      return Ok(await _reflectionService.GetReflectionCaptureById(id));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("All")]
    public async Task<ActionResult<ServiceResponse<List<GetUserReflectionDto>>>> GetAllReflections()
    {
      return Ok(await _reflectionService.GetAllReflections());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant, Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetReflectionDto>>> GetReflectionById(Guid id)
    {
      return Ok(await _reflectionService.GetReflectionById(id));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflection"></param>
    /// <returns></returns>
    [Authorize(Roles = "Participant")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<GetReflectionDto>>> PostReflection(AddReflectionStringDataDto reflection)
    {
      return Ok(await _reflectionService.PostReflection(reflection));
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
      return Ok(await _reflectionService.UpdateReflectionComment(updatedComment));
    }
  }
}
