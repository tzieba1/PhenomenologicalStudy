﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhenomenologicalStudy.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using System.Net;

namespace PhenomenologicalStudy.API.Controllers
{ 
  [Authorize]
  [Route("api/[controller]")] // api/authentication
  [ApiController]
  public class AuthenticationController : ControllerBase
  {
    private readonly IAuthService _authService;

    public AuthenticationController(
      IAuthService authService)
    {
      _authService = authService; // Authentication Service does most of the heavy lifting.
    }

    /// <summary>
    /// Endpoint for user to register and recieve an email confirmation link.
    /// </summary>
    /// <param name="user">Candidate user being registered</param>
    /// <returns>AuthenticationResult with status of email confirmation, otherwise BadRequest</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    public async Task<ActionResult<ServiceResponse<Guid>>> Register(RegisterUserDto user)
    {
      ServiceResponse<Guid> registerResponse = await _authService.Register(user);
      return registerResponse.Status switch
      {
        HttpStatusCode.BadRequest => BadRequest(registerResponse),
        HttpStatusCode.OK => Ok(registerResponse),
        _ => BadRequest(new ServiceResponse<Guid>()
        {
          Messages = new List<string>() { "Invalid payload." },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }

    /// <summary>
    /// Endpoint for user to confirm their email (only reachable by link sent to registerating email with security token attached).
    /// </summary>
    /// <param name="userId">Guid of user whos email is being confirmed</param>
    /// <param name="code">Security token generated by _userManager</param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("ConfirmEmail")]
    public async Task<ServiceResponse<AuthenticationTokens>> ConfirmEmail([FromQuery]string userId, [FromQuery] string code)
    {
      return await _authService.ConfirmEmail(userId, code);
    }

    /// <summary>
    /// Endpoint for user to login and recieve JWT including both token and refresh token properties.
    /// </summary>
    /// <param name="user">Login request POCO representing user credentials</param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    public async Task<ActionResult<ServiceResponse<AuthenticationTokens>>> Login(LoginUserDto user)
    {
      ServiceResponse<AuthenticationTokens> loginResponse = await _authService.Login(user);
      return loginResponse.Status switch
      {
        HttpStatusCode.OK => Ok(loginResponse),
        HttpStatusCode.NotFound => NotFound(loginResponse),
        HttpStatusCode.BadRequest => BadRequest(loginResponse),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, loginResponse),
        _ => BadRequest(new ServiceResponse<AuthenticationTokens>()
        {
          Messages = new List<string>() { "Invalid payload" },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }

    /// <summary>
    /// Endpoint for user to logout where response data contains Id of user whos refresh token has been revoked.
    /// </summary>
    /// <param name="tokens">Logout request POCO representing a an access token and refresh token</param>
    /// <returns></returns>
    [HttpPost]
    [Route("Logout")]
    public async Task<ActionResult<ServiceResponse<AuthenticationTokens>>> Logout(AuthenticationTokens tokens)
    {
      ServiceResponse<AuthenticationTokens> loginResponse = await _authService.Logout(tokens);
      return loginResponse.Status switch
      {
        HttpStatusCode.OK => Ok(loginResponse),
        HttpStatusCode.NotFound => NotFound(loginResponse),
        _ => BadRequest(new ServiceResponse<AuthenticationTokens>()
        {
          Messages = new List<string>() { "Invalid payload" },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }

    /// <summary>
    /// Endpoint used to refresh JWTs for users making requests to other API endpoints
    /// </summary>
    /// <param name="token"></param>
    /// <returns>BadRequest if Jwt canot be refreshed, otherwise refreshed Jwt</returns>
    [HttpPost]
    [Route("RefreshToken")]
    public async Task<ActionResult<ServiceResponse<AuthenticationTokens>>> RefreshToken(AuthenticationTokens tokens)
    {
      var refreshTokenResponse = await _authService.RefreshToken(tokens);
      return refreshTokenResponse.Status switch
      {
        HttpStatusCode.OK => Ok(refreshTokenResponse),
        HttpStatusCode.NotFound => NotFound(refreshTokenResponse),
        HttpStatusCode.BadRequest => BadRequest(refreshTokenResponse),
        HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, refreshTokenResponse),
        HttpStatusCode.Forbidden => StatusCode((int)HttpStatusCode.Forbidden, refreshTokenResponse),
        HttpStatusCode.Gone => StatusCode((int)HttpStatusCode.Gone, refreshTokenResponse),
        HttpStatusCode.Conflict => StatusCode((int)HttpStatusCode.Conflict, refreshTokenResponse),
        _ => BadRequest(new ServiceResponse<AuthenticationTokens>()
        {
          Messages = new List<string>() { "Invalid payload" },
          Success = false,
          Status = HttpStatusCode.BadRequest
        }),
      };
    }
  }
}