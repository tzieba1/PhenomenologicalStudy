using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using PhenomenologicalStudy.API.Models.ManyToMany;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services
{
  public class EmotionService : IEmotionService
  {
    private readonly PhenomenologicalStudyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;

    public EmotionService(PhenomenologicalStudyContext db, IMapper mapper, IAuthService authService, UserManager<User> userManager)
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
    public async Task<ServiceResponse<GetEmotionDto>> DeleteEmotionById(Guid id)
    {
      ServiceResponse<GetEmotionDto> serviceResponse = new();
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

        // Attempt to find emotion.
        Emotion emotion = bearerRoles.Contains("Admin") ?
          await _db.Emotions.Include(e => e.ReflectionChild)
                              .FirstOrDefaultAsync(e => e.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Emotions.Include(e => e.ReflectionChild)
                              .Where(e => e.ReflectionChild.Reflection.User.Id == bearerId)
                              .FirstOrDefaultAsync(e => e.Id == id)
          : null;

        // Check if emotion is found.
        if (emotion == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add($"Emotion with id {id} not found.");
          return serviceResponse;
        }

        // Remove emotion and Map to DTO for service response.
        _db.Emotions.Remove(emotion);
        await _db.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetEmotionDto>(emotion);
        serviceResponse.Messages.Add($"Successfully deleted emotion with id {id}");
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
    public async Task<ServiceResponse<GetEmotionDto>> GetEmotionById(Guid id)
    {
      ServiceResponse<GetEmotionDto> serviceResponse = new();
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

        // Attempt to find emotion.
        Emotion emotion = bearerRoles.Contains("Admin") ?
          await _db.Emotions.Include(e => e.ReflectionChild)
                            .FirstOrDefaultAsync(e => e.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Emotions.Include(e => e.ReflectionChild)
                            .Where(e => e.ReflectionChild.Reflection.User.Id == bearerId)
                            .FirstOrDefaultAsync(e => e.Id == id)
          : null;

        // Check if emotion is found.
        if (emotion == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find Emotion with id {id}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map Emotion to DTO
        serviceResponse.Data = _mapper.Map<GetEmotionDto>(emotion);
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
    public async Task<ServiceResponse<List<GetEmotionDto>>> GetEmotions()
    {
      ServiceResponse<List<GetEmotionDto>> serviceResponse = new();
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

        // Retrieve either all emotions as admin or only emotions related to user as participant
        List<Emotion> emotions = bearerRoles.Contains("Admin") ?
          await _db.Emotions.Include(e => e.ReflectionChild)
                            .ToListAsync()
          : bearerRoles.Contains("Participant") ?
          await _db.Emotions.Include(e => e.ReflectionChild)
                            .Where(e => e.ReflectionChild.Reflection.User.Id == bearerId) 
                            .ToListAsync()
          : null;

        // Check if any emotions are found
        if (emotions.Count == 0)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find any emotions for user with id {bearerId}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map emotions to DTO 
        serviceResponse.Data = _mapper.Map<List<GetEmotionDto>>(emotions);
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
    /// <param name="emotion"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> PostEmotion(AddEmotionDto emotion, Guid? reflectionChildId)
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

        // Retrieve ReflectionChild
        ReflectionChild reflectionChild = await _db.ReflectionChildren
          .Include(rc => rc.Reflection)
          .Include(rc => rc.Child)
          .SingleOrDefaultAsync(rc => rc.Id == reflectionChildId);

        // Check ReflectionChild exists (verify child is related to reflection)
        if (reflectionChild == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"ReflectionChild with id {reflectionChildId} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Check that bearer is not related to the relfection/child being related to created emotion
        if (!bearer.Reflections.Contains(reflectionChild.Reflection))
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Must retrieve related entities and relate to new emotion being posted via assignment
        Emotion newEmotion = _mapper.Map<Emotion>(emotion);
        newEmotion.ReflectionChild = reflectionChild;

        // Add emotion - at this point regardless of reflection ownership when bearer is Admin or based on authorized user when bearer is Participant
        EntityEntry<Emotion> addedEmotion = await _db.Emotions.AddAsync(newEmotion);
        await _db.SaveChangesAsync();
        serviceResponse.Data = addedEmotion.Entity.Id;
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
    /// <param name="updatedEmotion"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetEmotionDto>> PutEmotion(UpdateEmotionDto updatedEmotion)
    {
      ServiceResponse<GetEmotionDto> serviceResponse = new();
      try
      {
        Emotion emotion = await _db.Emotions
          .Include(e => e.ReflectionChild)
          .FirstOrDefaultAsync(e => e.Id == updatedEmotion.Id);

        // Check if emotion does not exist
        if (emotion == null)
        {
          serviceResponse.Messages.Add($"Emotion with id {updatedEmotion.Id} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Attempt to retrieve related ReflectionChild and check if not found
        ReflectionChild reflectionChild = await _db.ReflectionChildren
          .Include(rc => rc.Reflection)
          .FirstOrDefaultAsync(rc => rc.Id == emotion.ReflectionChild.Id);
        if (reflectionChild == null)
        {
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add("Related reflection child not found.");
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Attempt to retrieve users with same reflection as ReflectionChild found
        User user = await _db.Users
          .Include(u => u.Reflections)
          .FirstOrDefaultAsync(u => u.Reflections
            .FirstOrDefault(r => r.Children
              .FirstOrDefault(c => c.Id == reflectionChild.Id) 
            != null) 
          != null);

        // Check that related user is not found
        if (user == null)
        {
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add("Related user not found.");
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Check that bearer has retrieved Emotion's related User Id
        if (user.Id == _authService.GetUserId())
        {
          emotion.Type = updatedEmotion.Type;
          emotion.Intensity = updatedEmotion.Intensity;
          emotion.UpdatedTime = DateTimeOffset.UtcNow;
          await _db.SaveChangesAsync();
          serviceResponse.Data = _mapper.Map<GetEmotionDto>(emotion);
          serviceResponse.Status = HttpStatusCode.Created;
        }
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
