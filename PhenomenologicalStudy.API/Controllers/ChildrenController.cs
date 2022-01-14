using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(Roles = "Participant, Admin")]
  public class ChildrenController : ControllerBase
  {
    private readonly IChildService _childService;

    public ChildrenController(IChildService childService)
    {
      _childService = childService;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="child"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetChildDto>>> PutChild(UpdateChildDto child, [FromQuery] Guid? userId)
    {
      ServiceResponse<GetChildDto> response = await _childService.PutChild(child);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetChildDto>>> GetChildById(Guid id)
    {
      ServiceResponse<GetChildDto> response = await _childService.GetChildById(id);
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
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetChildDto>>>> GetChildren()
    {
      ServiceResponse<List<GetChildDto>> response = await _childService.GetChildren();
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
    /// <param name="child"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostChild(AddChildDto child, [FromQuery] Guid? userId)
    {
      ServiceResponse<Guid> response = await _childService.PostChild(child, userId);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
        HttpStatusCode.NotFound => NotFound(response),
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
    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<GetChildDto>>> DeleteChild(Guid id)
    {
      ServiceResponse<GetChildDto> response = await _childService.DeleteChildById(id);
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.NotFound => NotFound(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }
  }
}
