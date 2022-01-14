using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Badge;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services
{
  public class BadgeService : IBadgeService
  {
    private readonly PhenomenologicalStudyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;

    public BadgeService(PhenomenologicalStudyContext db, IMapper mapper, IAuthService authService, UserManager<User> userManager)
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
    public async Task<ServiceResponse<GetBadgeDto>> DeleteBadgeById(Guid id)
    {
      ServiceResponse<GetBadgeDto> serviceResponse = new();
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

        // Attempt to find badge.
        Badge badge = bearerRoles.Contains("Admin") ?
          await _db.Badges.Include(b => b.User)
                              .FirstOrDefaultAsync(b => b.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Badges.Include(b => b.User)
                              .Where(b => b.User.Id == bearerId)
                              .FirstOrDefaultAsync(b => b.Id == id)
          : null;

        // Check if badge is not found.
        if (badge == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add($"Badge with id {id} not found.");
          return serviceResponse;
        }

        // Remove badge and Map to DTO for service response.
        _db.Badges.Remove(badge);
        await _db.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetBadgeDto>(badge);
        serviceResponse.Messages.Add($"Successfully deleted badge with id {id}");
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
    public async Task<ServiceResponse<GetBadgeDto>> GetBadgeById(Guid id)
    {
      ServiceResponse<GetBadgeDto> serviceResponse = new();
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

        // Attempt to find badge.
        Badge badge = bearerRoles.Contains("Admin") ?
          await _db.Badges.Include(b => b.User)
                            .FirstOrDefaultAsync(b => b.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Badges.Include(b => b.User)
                            .Where(b => b.User.Id == bearerId)
                            .FirstOrDefaultAsync(b => b.Id == id)
          : null;

        // Check if badge is found.
        if (badge == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find Badge with id {id}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map Badge to DTO
        serviceResponse.Data = _mapper.Map<GetBadgeDto>(badge);
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
    public async Task<ServiceResponse<List<GetBadgeDto>>> GetBadges()
    {
      ServiceResponse<List<GetBadgeDto>> serviceResponse = new();
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

        // Retrieve either all badges as admin or only badges related to user as participant
        List<Badge> badges = bearerRoles.Contains("Admin") ?
          await _db.Badges.Include(b => b.User)
                            .ToListAsync()
          : bearerRoles.Contains("Participant") ?
          await _db.Badges.Include(b => b.User)
                            .Where(b => b.User.Id == bearerId)
                            .ToListAsync()
          : null;

        // Check if any badges are found
        if (badges.Count == 0)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find any badges for user with id {bearerId}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map badges to DTO 
        serviceResponse.Data = _mapper.Map<List<GetBadgeDto>>(badges);
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
    /// <param name="badge"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> PostBadge(AddBadgeDto badge, Guid? userId)
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
          .Include(u => u.Reflections)
          .Include(u => u.Questionnaires)
          .Include(u => u.Badges)
          .SingleOrDefaultAsync(u => u.Id == userId);

        // Check User exists 
        if (user == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"User with id {userId} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // After mapping to a Badge, must assign related user to new badge being posted via assignment
        Badge newBadge = _mapper.Map<Badge>(badge);
        newBadge.User = bearer;

        // Add badge - only Participants can add these
        EntityEntry<Badge> addedBadge = await _db.Badges.AddAsync(newBadge);
        await _db.SaveChangesAsync();
        serviceResponse.Data = addedBadge.Entity.Id;
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
    /// <param name="badge"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetBadgeDto>> PutBadge(UpdateBadgeDto updatedBadge)
    {
      ServiceResponse<GetBadgeDto> serviceResponse = new();
      try
      {
        Badge badge = await _db.Badges
          .Include(b => b.User)
          .FirstOrDefaultAsync(b => b.Id == updatedBadge.Id);

        // Check if badge does not exist
        if (updatedBadge == null)
        {
          serviceResponse.Messages.Add($"Badge with id {updatedBadge.Id} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Check if user of retrieved badge is not bearer
        if (badge.User.Id != _authService.GetUserId())
        {
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add("Questionnair not found.");
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Update badge, save changes in DB, and map to service response
        badge.Value = updatedBadge.Value;
        badge.Message = updatedBadge.Message;
        badge.UpdatedTime = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetBadgeDto>(badge);
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
