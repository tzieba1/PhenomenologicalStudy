using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services
{
  public class ChildService : IChildService
  {
    private readonly PhenomenologicalStudyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;

    public ChildService(PhenomenologicalStudyContext db, IMapper mapper, IAuthService authService, UserManager<User> userManager)
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
    public async Task<ServiceResponse<GetChildDto>> DeleteChildById(Guid id)
    {
      ServiceResponse<GetChildDto> serviceResponse = new();
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

        // Attempt to find child.
        Child child = bearerRoles.Contains("Admin") ?
          await _db.Children.Include(c => c.Reflection)
                            .Include(c => c.ReflectionChild)
                            .Include(c => c.User)
                            .FirstOrDefaultAsync(c => c.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Children.Include(c => c.Reflection)
                            .Include(c => c.ReflectionChild)
                            .Include(c => c.User)
                            .Where(c => c.User.Id == bearerId)
                            .FirstOrDefaultAsync(c => c.Id == id)
          : null;

        // Check if child is found.
        if (child == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add($"Child with id {id} not found.");
          return serviceResponse;
        }

        // Remove child and Map to DTO for service response based on role
        _db.Children.Remove(child);
        await _db.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetChildDto>(child);
        serviceResponse.Messages.Add($"Successfully deleted Child with id {id}");
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
    public async Task<ServiceResponse<GetChildDto>> GetChildById(Guid id)
    {
      ServiceResponse<GetChildDto> serviceResponse = new();
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

        // Retrieve either any child as admin or only child related to user as participant
        Child child = bearerRoles.Contains("Admin") ?
          await _db.Children.Include(c => c.ReflectionChild)
                            .Include(c => c.Reflection)
                            .Include(c => c.User)
                            .FirstOrDefaultAsync(e => e.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Children.Include(c => c.ReflectionChild)
                            .Include(c => c.Reflection)
                            .Include(c => c.User)
                            .Where(c => c.User.Id == bearerId)
                            .FirstOrDefaultAsync(e => e.Id == id)
          : null;


        // Check if child is found.
        if (child == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find Child with id {id}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map Child to DTO
        serviceResponse.Data = _mapper.Map<GetChildDto>(child);
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
    public async Task<ServiceResponse<List<GetChildDto>>> GetChildren()
    {
      ServiceResponse<List<GetChildDto>> serviceResponse = new();
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

        // Retrieve either all children as admin or only children related to user as participant
        List<Child> children = bearerRoles.Contains("Admin") ?
          await _db.Children.Include(c => c.ReflectionChild)
                            .Include(c => c.Reflection)
                            .Include(c => c.User)
                            .ToListAsync()
          : bearerRoles.Contains("Participant") ?
          await _db.Children.Include(c => c.ReflectionChild)
                            .Include(c => c.Reflection)
                            .Include(c => c.User)
                            .Where(c => c.User.Id == bearerId)
                            .ToListAsync()
          : null;

        // Check if any children are found
        if (children.Count == 0)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find any children for user with id {bearerId}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map children to DTO 
        serviceResponse.Data = _mapper.Map<List<GetChildDto>>(children);
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
    /// <param name="child"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> PostChild(AddChildDto child, Guid? userId)
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

        // Map request body to a new Child
        Child newChild = _mapper.Map<Child>(child);

        // Attempt to retrieve related user 
        User relatedUser;

        // Admins must supply a userId (using optional parameter)
        if (bearerRoles.Contains("Admin"))
        {
          relatedUser = await _userManager.FindByIdAsync(userId.ToString());

          // Check if user exists
          if (relatedUser == null)
          {
            serviceResponse.Success = false;
            serviceResponse.Messages.Add($"User with id {userId} not found.");
            serviceResponse.Status = HttpStatusCode.NotFound;
            return serviceResponse;
          }

          // Assign user specified by Admin to be related to new child being created
          newChild.User = relatedUser;
        }
        
        // Add relationship between child being added and retrieved user.
        newChild.User = bearer;

        // Add emotion to database, track changes with EntityEntry<T> to access database generated Id and save changes.
        EntityEntry<Child> addedChild = await _db.Children.AddAsync(newChild);
        await _db.SaveChangesAsync();
        serviceResponse.Data = addedChild.Entity.Id;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="updatedChild"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetChildDto>> PutChild(UpdateChildDto updatedChild)
    {
      ServiceResponse<GetChildDto> serviceResponse = new();
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

        // Retrieve child using DTO
        Child child = await _db.Children
          .Include(c => c.User)
          .FirstOrDefaultAsync(c => c.Id == updatedChild.Id);

        // Check if child does not exist
        if (child == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Check that Child does not belong to bearer when beaer has Participant role.
        if (child.User.Id != bearer.Id && bearerRoles.Contains("Participant"))
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Assign updated properties to retrieved Child
        child.FirstName = updatedChild.FirstName;
        child.LastName = updatedChild.LastName;
        child.DateOfBirth = updatedChild.DateOfBirth;
        child.Gender = updatedChild.Gender;
        child.UpdatedTime = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();

        // Map child to service request
        serviceResponse.Data = _mapper.Map<GetChildDto>(child);
        serviceResponse.Messages.Add($"Successfully updated child with id {child.Id}");
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
