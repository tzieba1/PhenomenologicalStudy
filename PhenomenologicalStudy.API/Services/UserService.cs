using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Questionnaire;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services
{
  public class UserService : IUserService
  {
    private readonly PhenomenologicalStudyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UserService(PhenomenologicalStudyContext db, IMapper mapper, IAuthService authService, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
      _db = db;
      _mapper = mapper;
      _authService = authService;
      _userManager = userManager;
      _roleManager = roleManager;
    }

    /// <summary>
    /// Service logic for UsersController endpoint 'DELETE: /api/Users/{id}'.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Service response as a User Data Transfer Object.</returns>
    public async Task<ServiceResponse<GetUserDto>> DeleteUserById(Guid id)
    {
      ServiceResponse<GetUserDto> serviceResponse = new();
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

        // Attempt to find user.
        User user = bearerRoles.Contains("Admin") ?
          await _db.Users
            .Include(c => c.Children)
            .Include(c => c.Reflections)
            .FirstOrDefaultAsync(c => c.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Users
            .Include(c => c.Children)
            .Include(c => c.Reflections)
            .Where(c => c.Id == bearerId)
            .FirstOrDefaultAsync(e => e.Id == id)
          : null;


        // Check if user not found.
        if (user == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add($"User with id {id} not found.");
          return serviceResponse;
        }

        // Remove user and Map to DTO for service response.
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetUserDto>(user);
        serviceResponse.Messages.Add($"Successfully deleted user with id {id}");
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
    /// Service logic for UsersController endpoint 'GET: /api/Users/{id}'.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Service response as a User Data Transfer Object.</returns>
    public async Task<ServiceResponse<GetUserDto>> GetUserById(Guid id)
    {
      ServiceResponse<GetUserDto> serviceResponse = new();
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

        // Attempt to find user.
        User user = bearerRoles.Contains("Admin") ?
          await _db.Users.FirstOrDefaultAsync(u => u.Id == id)
          : bearerRoles.Contains("Participant") ?
          await _db.Users.Where(u => u.Id == bearerId)
                         .FirstOrDefaultAsync(e => e.Id == id)
          : null;

        // Check if user is found.
        if (user == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Could not find User with id {id}");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Map User to DTO
        serviceResponse.Data = _mapper.Map<GetUserDto>(user);
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
    /// Service logic for UsersController endpoint 'GET: /api/Users/Questionnaires'.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<List<GetQuestionnaireDto>>> GetUserQuestionnaires(Guid? id)
    {
      ServiceResponse<List<GetQuestionnaireDto>> serviceResponse = new();
      try
      {
        // Check if the user exists
        User user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"User with id {id} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Get all Questionnaires for a specific user.
        serviceResponse.Data = id == null ?
          await _db.Questionnaires
            .Include(q => q.User)
            .Select(q => _mapper.Map<GetQuestionnaireDto>(q))
            .ToListAsync()
          : await _db.Questionnaires
            .Include(q => q.User)
            .Where(q => q.User.Id == id)
            .Select(q => _mapper.Map<GetQuestionnaireDto>(q))
            .ToListAsync();
        serviceResponse.Messages.Add($"Successfully retrieved all questionnaires for user with id {id}");
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
    public async Task<ServiceResponse<List<GetReflectionDto>>> GetUserReflections(Guid? id)
    {
      ServiceResponse<List<GetReflectionDto>> serviceResponse = new();
      try
      {
        // Check if the user exists
        User user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"User with id {id} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Get all reflections and included data for a specific user.
        List<GetReflectionDto> reflections = id == null ?
          await _db.Reflections
          .Include(r => r.User)
          .Include(r => r.Capture)
          .Include(r => r.Children)
          .Include(r => r.Comment)
          .Select(r => _mapper.Map<GetReflectionDto>(r))
          .ToListAsync()
        : await _db.Reflections
          .Include(r => r.User)
          .Include(r => r.Capture)
          .Include(r => r.Children)
          .Include(r => r.Comment)
          .Where(r => r.User.Id == id)
          .Select(r => _mapper.Map<GetReflectionDto>(r))
          .ToListAsync();


        // Add mapped User, Child, and List<Emotion> entities to each ReflectionChild returned in service response data
        foreach (GetReflectionDto reflection in reflections)
        {
          // Use reflection Id to recover user Id
          Reflection userReflection = await _db.Reflections.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == reflection.Id);
          reflection.User = _mapper.Map<GetUserDto>(await _db.Users.FirstOrDefaultAsync(u => u.Id == userReflection.User.Id));
          foreach (var reflectionChild in reflection.Children)
          {
            // Retrieve child and emotion
            GetChildDto child = _mapper.Map<GetChildDto>(await _db.Children.FirstOrDefaultAsync(c => c.ReflectionChild.Id == reflectionChild.Id));
            ICollection<GetEmotionDto> emotions = _mapper.Map<List<GetEmotionDto>>(await _db.Emotions.Where(e => e.ReflectionChild.Id == reflectionChild.Id).ToListAsync());

            // Assign child and emotion to ReflectionChild entity
            reflectionChild.Emotions = emotions;
            reflectionChild.Child = child;
          }
        }
        await _db.SaveChangesAsync();
        serviceResponse.Data = reflections;
        serviceResponse.Messages.Add($"Successfully retrieved all reflections for user with id {id}");
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
    public async Task<ServiceResponse<List<GetUserRoleDto>>> GetUserRoles(Guid? id)
    {
      ServiceResponse<List<GetUserRoleDto>> serviceResponse = new();
      try
      {
        // Retrieve user from bearer's userId claim
        Guid bearerId = _authService.GetUserId();
        User bearer = await _userManager.FindByIdAsync(bearerId.ToString());

        // Check if bearer exists
        if (bearer == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Retrieve bearer roles in single use of db context
        IList<string> bearerRoles = await _userManager.GetRolesAsync(bearer);

        // Check that bearer is admin to retrieve all roles
        List<Role> retrievedRoles = new();
        if (bearerRoles.Contains("Admin"))
        {
          // Whenever a user id is provided in query string params, attempt to find user and check if not found
          if (id != null)
          {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
              serviceResponse.Success = false;
              serviceResponse.Messages.Add($"User with id {id} not found.");
              serviceResponse.Status = HttpStatusCode.NotFound;
              return serviceResponse;
            }

            // Only retrieve roles for the user with id provided
            IList<string> iListRoles = await _userManager.GetRolesAsync(user);
            foreach (string roleName in iListRoles)
            {
              retrievedRoles.Add(await _roleManager.FindByNameAsync(roleName));
            }
          }
          else
          {
            retrievedRoles = await _db.Roles.ToListAsync();
          }
        }
        // Check that bearer is Participant to retrieve only their roles
        else if (bearerRoles.Contains("Participant"))
        {
          List<IdentityUserRole<Guid>> userRoles = await _db.UserRoles
            .Where(r => r.UserId == bearerId)
            .ToListAsync();

          // Attempt to find user role relationship by getting the Id for it
          //IdentityUserRole<Guid> userRole = userRoles.FirstOrDefault(ur => ur.)

          // Only retrieve roles for the participant 
          retrievedRoles = await _db.Roles.Where(r => userRoles.FirstOrDefault(ur => ur.RoleId == r.Id) != null).ToListAsync();
        }

        // Map roles to DTO 
        serviceResponse.Data = _mapper.Map<List<GetUserRoleDto>>(retrievedRoles);
        serviceResponse.Messages.Add("Successfully retrieved roles.");
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
    public async Task<ServiceResponse<List<GetUserDto>>> GetUsers()
    {
      ServiceResponse<List<GetUserDto>> serviceResponse = new();
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

        // Retrieve either all users as admin or only users related to user as participant
        List<User> users = bearerRoles.Contains("Admin") ?
          await _db.Users.ToListAsync()
          : bearerRoles.Contains("Participant") ?
          await _db.Users.Where(u => u.Id == bearerId)
                         .ToListAsync()
          : null;

        // Map users to DTO 
        serviceResponse.Data = _mapper.Map<List<GetUserDto>>(users);
        serviceResponse.Messages.Add("User(s) found successfully.");
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
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> PostUser(AddUserDto user)
    {
      ServiceResponse<Guid> serviceResponse = new();
      try
      {
        // Check if user exists 
        if ((await _userManager.FindByEmailAsync(user.Email)) != null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("User already exists with that email address.");
          serviceResponse.Status = HttpStatusCode.Conflict;
          return serviceResponse;
        }

        // Create new user (bearer performing POST is authenticated Admin at this point)
        User newUser = new()
        {
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName,
          EmailConfirmed = true,
          UserName = user.Email
        };
        IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
        if (!result.Succeeded)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Could not successfully create new user.");
          serviceResponse.Status = HttpStatusCode.InternalServerError;
          return serviceResponse;
        }

        // Retrieve newly created user to access Id
        User addedUser = await _userManager.FindByEmailAsync(user.Email);

        // Assign user to Particiapnt role
        result = await _userManager.AddToRoleAsync(addedUser, "Participant");
        if (!result.Succeeded)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Could not successfully add Participant role to user.");
          serviceResponse.Status = HttpStatusCode.InternalServerError;
          return serviceResponse;
        }

        // Add "Participant" claim to roles
        result = await _userManager.AddClaimAsync(addedUser, new Claim(ClaimTypes.Role, "Participant"));
        if (!result.Succeeded)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Could not successfully add Participant role claim to user.");
          serviceResponse.Status = HttpStatusCode.InternalServerError;
          return serviceResponse;
        }

        // Save changes and add user Id to service response.
        await _db.SaveChangesAsync();
        serviceResponse.Data = newUser.Id;
        serviceResponse.Messages.Add($"Successfully registered new user with email/username {addedUser.Email}.");
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
    /// <param name="newRole"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<List<GetUserRoleDto>>> PostUserRole(AddUserRoleDto newRole)
    {
      ServiceResponse<List<GetUserRoleDto>> serviceResponse = new();
      try
      {
        // Retrieve user from bearer's userId claim
        Guid bearerId = _authService.GetUserId();
        User bearer = await _userManager.FindByIdAsync(bearerId.ToString());

        // Check if bearer exists
        if (bearer == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Retrieve bearer roles in single use of db context
        IList<string> bearerRoles = await _userManager.GetRolesAsync(bearer);

        // Check that bearer is not an Admin
        if (!bearerRoles.Contains("Admin"))
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          return serviceResponse;
        }

        // Attempt to retrieve user and check that user does not exist
        User user = await _userManager.FindByIdAsync(newRole.UserId.ToString());
        if (user == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"User does not exist.");
          serviceResponse.Status = HttpStatusCode.Conflict;
          return serviceResponse;
        }

        // Check that role exists
        if ((await _db.Roles.FirstOrDefaultAsync(r => r.Name == newRole.Name)) != null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Role with name {newRole.Name} already exists.");
          serviceResponse.Status = HttpStatusCode.Conflict;
          return serviceResponse;
        }

        // Create role in database and check if not successful
        IdentityResult addRole = await _roleManager.CreateAsync(_mapper.Map<Role>(newRole));
        if (!addRole.Succeeded)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.InternalServerError;
          serviceResponse.Messages.Add("Creating a new role was not successful");
          return serviceResponse;
        }

        // Add user to new role and check if not successful
        IdentityResult addUserToRole = await _userManager.AddToRoleAsync(user, newRole.Name);
        if (!addUserToRole.Succeeded)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.InternalServerError;
          serviceResponse.Messages.Add("Assigning user to newly created role was not successful.");
          return serviceResponse;
        }

        // Only retrieve roles for the user and map to DTO
        List<IdentityUserRole<Guid>> userRoles = await _db.UserRoles
            .Where(r => r.UserId == user.Id)
            .ToListAsync();
        serviceResponse.Data = _mapper.Map<List<GetUserRoleDto>>(await _db.Roles
          .Where(r => userRoles.FirstOrDefault(ur => ur.RoleId == r.Id) != null)
          .ToListAsync());
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
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetUserDto>> PutUser(UpdateUserDto updatedUser)
    {
      ServiceResponse<GetUserDto> serviceResponse = new();
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

        User user = bearerRoles.Contains("Admin") ?
          await _db.Users.FirstOrDefaultAsync(u => u.Id == updatedUser.Id)
          : bearerRoles.Contains("Participant") ?
          await _db.Users.Where(u => u.Id == bearerId).FirstOrDefaultAsync(u => u.Id == updatedUser.Id)
          : null;

        // Check if user does not exist
        if (user == null)
        {
          serviceResponse.Messages.Add($"User with id {updatedUser.Id} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Check that bearer has retrieved User's related User Id
        if (user.Id != _authService.GetUserId() && bearerRoles.Contains("Participant"))
        {
          serviceResponse.Messages.Add($"Unauthorized.");
          serviceResponse.Status = HttpStatusCode.Unauthorized;
          serviceResponse.Success = false;
          return serviceResponse;
        }

        // Update the user now that either correct participant or an admin has ben authenticated
        user.Email = updatedUser.Email;
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.LastName = updatedUser.LastName;

        // Check that bearer is admin to be able to update the role
        if (bearerRoles.Contains("Admin"))
        {
          // Check the role being updated exists
          if ((await _db.Roles.FirstOrDefaultAsync(r => r.Name == updatedUser.Role)) != null)
          {
            //Check that role being added does not exist for user
            if (!(await _userManager.IsInRoleAsync(user, updatedUser.Role)))
            {
              IdentityResult addRole = await _userManager.AddToRolesAsync(user, new List<string> { updatedUser.Role });
              if (!addRole.Succeeded)
              {
                serviceResponse.Messages.Add("Did not succeed updating user role.");
              }
            }
            else
            {
              serviceResponse.Messages.Add($"User already has the {updatedUser.Role} role.");
            }
          }
          else
          {
            serviceResponse.Messages.Add($"Role named {updatedUser.Role} does not exist.");
          }
        }
        else
        {
          serviceResponse.Messages.Add("Must be an Admin to update a role. All other fields updated.");
        }

        // Save changes then map updated user to DTO
        await _db.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetUserDto>(user);
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
