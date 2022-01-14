using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Badge;
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
  public class BadgesController : ControllerBase
  {
    private readonly IBadgeService _badgeService;

    public BadgesController(IBadgeService badgeService)
    {
      _badgeService = badgeService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="badge"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetBadgeDto>>> GetBadgeById(Guid id)
    {
      ServiceResponse<GetBadgeDto> response = await _badgeService.GetBadgeById(id);
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
    /// <param name="badge"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetBadgeDto>>>> GetBadges()
    {
      ServiceResponse<List<GetBadgeDto>> response = await _badgeService.GetBadges();
      return response.Status switch
      {
        HttpStatusCode.OK => Ok(response),
        HttpStatusCode.Unauthorized => Unauthorized(response),
        HttpStatusCode.Found => NotFound(response),
        HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
        _ => StatusCode((int)response.Status, (response))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="badge"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<Guid>>> PostBadge(AddBadgeDto badge, Guid? userId)
    {
      ServiceResponse<Guid> response = await _badgeService.PostBadge(badge, userId);
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<GetBadgeDto>>> DeleteBadge(Guid id)
    {
      ServiceResponse<GetBadgeDto> response = await _badgeService.DeleteBadgeById(id);
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
    /// <param name="badge"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<GetBadgeDto>>> PutBadge(UpdateBadgeDto badge)
    {
      ServiceResponse<GetBadgeDto> response = await _badgeService.PutBadge(badge);
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
