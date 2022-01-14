using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Questionnaire;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
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
  public class UsersController : ControllerBase
  {
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
      _userService = userService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetUserDto>>> GetUserById(Guid id)
    {
      ServiceResponse<GetUserDto> response = await _userService.GetUserById(id);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => BadRequest(response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetUserDto>>>> GetUsers()
    {
      ServiceResponse<List<GetUserDto>> response = await _userService.GetUsers();
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
    /// <param name="user"></param>
    /// <param name="reflectionChildId"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostUser(AddUserDto user)
    {
      ServiceResponse<Guid> response = await _userService.PostUser(user);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
        HttpStatusCode.Conflict => StatusCode((int)HttpStatusCode.Conflict, response),
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<GetUserDto>>> DeleteUser(Guid id)
    {
      ServiceResponse<GetUserDto> response = await _userService.DeleteUserById(id);
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
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetUserDto>>> PutUser(UpdateUserDto user)
    {
      ServiceResponse<GetUserDto> response = await _userService.PutUser(user);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    [HttpPost("Roles")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<List<GetUserRoleDto>>>> PostUserRole(AddUserRoleDto role)
    {
      ServiceResponse<List<GetUserRoleDto>> response = await _userService.PostUserRole(role);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
        HttpStatusCode.Conflict => StatusCode((int)HttpStatusCode.Conflict, response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Roles")]
    public async Task<ActionResult<ServiceResponse<List<GetUserRoleDto>>>> GetUserRoles([FromQuery] Guid? userId)
    {
      ServiceResponse<List<GetUserRoleDto>> response = await _userService.GetUserRoles(userId);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("Reflections")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<List<GetReflectionDto>>>> GetUserReflections([FromQuery] Guid? userId)
    {
      ServiceResponse<List<GetReflectionDto>> response = await _userService.GetUserReflections(userId);
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
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("Questionnaires")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<List<GetQuestionnaireDto>>>> GetUserQuestionnaires([FromQuery] Guid? userId)
    {
      ServiceResponse<List<GetQuestionnaireDto>> response = await _userService.GetUserQuestionnaires(userId);
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