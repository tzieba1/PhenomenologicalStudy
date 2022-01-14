using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Questionnaire;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services
{
  public class QuestionnaireService : IQuestionnaireService
  {
    private readonly PhenomenologicalStudyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;

    public QuestionnaireService(PhenomenologicalStudyContext db, IMapper mapper, IAuthService authService, UserManager<User> userManager)
    {
      _db = db;
      _mapper = mapper;
      _authService = authService;
      _userManager = userManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetQuestionnaireDto>> DeleteQuestionnaireById(Guid id)
    {
      ServiceResponse<GetQuestionnaireDto> serviceResponse = new();
      try
      {
        // Retrieve user from bearer's userId claim
        Guid bearerId = _authService.GetUserId();
        User bearer = await _userManager.FindByIdAsync(bearerId.ToString());

        // Check if user exists
        if (bearer == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Retrieve bearer roles in single use of db context
        IList<string> bearerRoles = await _userManager.GetRolesAsync(bearer);

        // Attempt to find questionnaire.
        PRFQuestionnaire questionnaire = bearerRoles.Contains("Admin") ?
          await _db.Questionnaires.Include(q => q.User)
                              .FirstOrDefaultAsync(q => q.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Questionnaires.Include(q => q.User)
                              .Where(q => q.User.Id == bearerId)
                              .FirstOrDefaultAsync(q => q.Id == id)
          : null;

        // Check if questionnaire is not found.
        if (questionnaire == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add($"Questionnaire with id {id} not found.");
          return serviceResponse;
        }

        // Remove questionnaire and Map to DTO for service response.
        _db.Questionnaires.Remove(questionnaire);
        await _db.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetQuestionnaireDto>(questionnaire);
        serviceResponse.Messages.Add($"Successfully deleted questionnaire with id {id}");
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Status = HttpStatusCode.InternalServerError;
        serviceResponse.Messages.Add(ex.Message);
      }
      return serviceResponse;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetQuestionnaireDto>> GetQuestionnaireById(Guid id)
    {
      ServiceResponse<GetQuestionnaireDto> serviceResponse = new();
      try
      {
        // Retrieve user from bearer's userId claim
        Guid bearerId = _authService.GetUserId();
        User bearer = await _userManager.FindByIdAsync(bearerId.ToString());

        // Check if user exists
        if (bearer == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Retrieve bearer roles in single use of db context
        IList<string> bearerRoles = await _userManager.GetRolesAsync(bearer);

        // Attempt to find questionnaire.
        PRFQuestionnaire questionnaire = bearerRoles.Contains("Admin") ?
          await _db.Questionnaires.Include(q => q.User)
                            .FirstOrDefaultAsync(q => q.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Questionnaires.Include(q => q.User)
                            .Where(q => q.User.Id == bearerId)
                            .FirstOrDefaultAsync(q => q.Id == id)
          : null;

        // Check if questionnaire is found.
        if (questionnaire == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find Questionnaire with id {id}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map PRFQuestionnaire to DTO
        serviceResponse.Data = _mapper.Map<GetQuestionnaireDto>(questionnaire);
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Status = HttpStatusCode.InternalServerError;
        serviceResponse.Messages.Add(ex.Message);
      }
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<List<GetQuestionnaireDto>>> GetQuestionnaires()
    {
      ServiceResponse<List<GetQuestionnaireDto>> serviceResponse = new();
      try
      {
        // Retrieve user from bearer's userId claim
        Guid bearerId = _authService.GetUserId();
        User bearer = await _userManager.FindByIdAsync(bearerId.ToString());

        // Check if user exists
        if (bearer == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Retrieve bearer roles in single use of db context
        IList<string> bearerRoles = await _userManager.GetRolesAsync(bearer);

        // Check if user has any roles
        if (bearerRoles.Count == 0)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Retrieve either all questionnaires as admin or only questionnaires related to user as participant
        List<PRFQuestionnaire> questionnaires = bearerRoles.Contains("Admin") ?
          await _db.Questionnaires.Include(q => q.User)
                            .ToListAsync()
          : bearerRoles.Contains("Participant") ?
          await _db.Questionnaires.Include(q => q.User)
                            .Where(q => q.User.Id == bearerId)
                            .ToListAsync()
          : null;

        // Check if any questionnaires are found
        if (questionnaires.Count == 0)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find any questionnaires for user with id {bearerId}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map questionnaires to DTO 
        serviceResponse.Data = _mapper.Map<List<GetQuestionnaireDto>>(questionnaires);
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Status = HttpStatusCode.InternalServerError;
        serviceResponse.Messages.Add(ex.Message);
      }
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="questionnaire"></param>
    /// <param name="reflectionChildId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> PostQuestionnaire(AddQuestionnaireDto questionnaire, Guid? userId)
    {
      ServiceResponse<Guid> serviceResponse = new();
      try
      {
        // Retrieve user from bearer's userId claim
        Guid bearerId = _authService.GetUserId();
        User bearer = await _userManager.FindByIdAsync(bearerId.ToString());

        // Check if user exists
        if (bearer == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Retrieve User
        User user = await _db.Users
          .Include(u => u.Questionnaires)
          .SingleOrDefaultAsync(u => u.Id == userId);

        // Check User exists 
        if (user == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"User with id {userId} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // After mapping to a PRFQuestionnaire, must assign related user to new questionnaire being posted via assignment
        PRFQuestionnaire newQuestionnaire = _mapper.Map<PRFQuestionnaire>(questionnaire);
        newQuestionnaire.User = bearer;

        // Add questionnaire - only Participants can add these
        EntityEntry<PRFQuestionnaire> addedQuestionnaire = await _db.Questionnaires.AddAsync(newQuestionnaire);
        await _db.SaveChangesAsync();
        serviceResponse.Data = addedQuestionnaire.Entity.Id;
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Status = HttpStatusCode.InternalServerError;
        serviceResponse.Messages.Add(ex.Message);
      }
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="questionnaire"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetQuestionnaireDto>> PutQuestionnaire(UpdateQuestionnaireDto updatedQuestionnaire)
    {
      ServiceResponse<GetQuestionnaireDto> serviceResponse = new();
      try
      {
        PRFQuestionnaire questionnaire = await _db.Questionnaires
          .Include(q => q.User)
          .FirstOrDefaultAsync(q => q.Id == updatedQuestionnaire.Id);

        // Check if questionnaire does not exist
        if (updatedQuestionnaire == null)
        {
          serviceResponse.Messages.Add($"Questionnaire with id {updatedQuestionnaire.Id} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Check if user of retrieved questionnaire is not bearer
        if (questionnaire.User.Id != _authService.GetUserId())
        {
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add("Questionnair not found.");
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Update questionnaire, save changes in DB, and map to service response
        questionnaire.Statement1 = updatedQuestionnaire.Statement1;
        questionnaire.Statement2 = updatedQuestionnaire.Statement2;
        questionnaire.Statement3 = updatedQuestionnaire.Statement3;
        questionnaire.Statement4 = updatedQuestionnaire.Statement4;
        questionnaire.Statement5 = updatedQuestionnaire.Statement5;
        questionnaire.Statement6 = updatedQuestionnaire.Statement6;
        questionnaire.Statement7 = updatedQuestionnaire.Statement7;
        questionnaire.Statement8 = updatedQuestionnaire.Statement8;
        questionnaire.Statement9 = updatedQuestionnaire.Statement9;
        questionnaire.Statement10 = updatedQuestionnaire.Statement10;
        questionnaire.Statement12 = updatedQuestionnaire.Statement12;
        questionnaire.Statement13 = updatedQuestionnaire.Statement13;
        questionnaire.Statement14 = updatedQuestionnaire.Statement14;
        questionnaire.Statement15 = updatedQuestionnaire.Statement15;
        questionnaire.Statement16 = updatedQuestionnaire.Statement16;
        questionnaire.Statement17 = updatedQuestionnaire.Statement17;
        questionnaire.Statement18 = updatedQuestionnaire.Statement18;
        questionnaire.UpdatedTime = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetQuestionnaireDto>(questionnaire);
        serviceResponse.Status = HttpStatusCode.Created;
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Status = HttpStatusCode.InternalServerError;
        serviceResponse.Messages.Add(ex.Message);
      }
      return serviceResponse;
    }
  }
}
