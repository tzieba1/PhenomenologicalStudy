using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Capture;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Comment;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection;
using PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using PhenomenologicalStudy.API.Models.ManyToMany;
using PhenomenologicalStudy.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services
{
  public class ReflectionService : IReflectionService
  {
    private readonly PhenomenologicalStudyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public ReflectionService(PhenomenologicalStudyContext db, IMapper mapper, IAuthService authService)
    {
      _db = db;
      _mapper = mapper;
      _authService = authService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> DeleteReflection(Guid id)
    {
      ServiceResponse<Guid> serviceResponse = new();
      try
      {
        Reflection reflection = await _db.Reflections.FindAsync(id);
        if (reflection != null)
        {
          _db.Reflections.Remove(reflection);
          await _db.SaveChangesAsync();
          serviceResponse.Data = id;
          serviceResponse.Messages.Add($"Reflection with id {id} deleted successfully.");
        }
        else
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add($"Reflection with id {id} not found.");
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

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="reflectionChild"></param>
    ///// <returns></returns>
    //public async Task<ServiceResponse<GetUserReflectionDto>> DeleteReflectionChildEmotion(Guid reflectionId, Guid emotionId)
    //{
    //  ServiceResponse<GetUserReflectionDto> serviceResponse = new();
    //  try
    //  {
    //    Guid bearerId = GetUserId();
          
    //    // --- Retrtieve the emotion to delete
    //    Emotion emotionToDelete = await _db.Emotions.FindAsync(emotionId);
    //    if (emotionToDelete != null)
    //    {
    //      _db.Emotions.Remove(emotionToDelete);
    //      await _db.SaveChangesAsync(); // Needed to delete pseudo 'asynchronously' where DbSet<T>.Remove queues a delete operation executed during SaveChanges() or SaveChangesAsync()
    //      serviceResponse.Messages.Add($"Emotion with id {emotionId} deleted successfully.");
    //      serviceResponse.Data = _mapper.Map<GetUserReflectionDto>(await _db.Reflections
    //        .Include(r => r.Comment)
    //        .Include(r => r.Children)
    //        .Where(r => r.User.Id == GetUserId() && r.Id == reflectionId)
    //        .FirstOrDefaultAsync(r => r.Id == reflectionId));
    //    }
    //    else
    //    {
    //      serviceResponse.Success = false;
    //      serviceResponse.Messages.Add("Emotion not found."); // No need for extra details.
    //      serviceResponse.Status = HttpStatusCode.NotFound;
    //    }
    //    //Reflection reflection = await _db.Reflections
    //    //  .Include(r => r.Capture)
    //    //  .Include(r => r.Comment)
    //    //  .Include(r => r.Children)
    //    //  .Where(r => r.User.Id == bearerId)
    //    //  .FirstOrDefaultAsync(r => r.Id == reflectionChild.ReflectionId);
    //    //serviceResponse.Data = _mapper.Map<GetUserReflectionDto>(); 
    //  }
    //  catch (Exception ex)
    //  {
    //    serviceResponse.Status = HttpStatusCode.InternalServerError;
    //    serviceResponse.Messages = new List<string>() { ex.Message };
    //    serviceResponse.Success = false;
    //  }
    //  return serviceResponse;
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<List<GetUserReflectionDto>>> GetUserReflections()
    {
      ServiceResponse<List<GetUserReflectionDto>> serviceResponse = new();
      try
      {
        Guid bearerId = _authService.GetUserId();
        serviceResponse.Data = await _db.Reflections
           .Include(r => r.Comment)
           .Include(r => r.Children)
           .Where(r => r.User.Id == bearerId)  // Only retrieve reflections for user logged in.
           .Select(r => _mapper.Map<GetUserReflectionDto>(r))
           .ToListAsync();
        serviceResponse.Messages.Add($"Successfully retrieved all reflections for user '{await _db.Users.FindAsync(bearerId)}'");

        // Add mapped Child and List<Emotion> entities to each ReflectionChild returned in service response data
        foreach (GetUserReflectionDto reflection in serviceResponse.Data)
        {
          await RetrieveReflectionChildAndEmotions(reflection.Children);
        }
        await _db.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        serviceResponse.Status = HttpStatusCode.InternalServerError;
        serviceResponse.Messages.Add(ex.Message);
        serviceResponse.Success = false;
      }
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<List<GetReflectionDto>>> GetAllReflections()
    {
      ServiceResponse<List<GetReflectionDto>> serviceResponse = new();
      try
      {
        Guid bearerId = _authService.GetUserId();
        serviceResponse.Data = await _db.Reflections
           .Include(r => r.Comment)
           .Include(r => r.Children)
           .Select(r => _mapper.Map<GetReflectionDto>(r))
           .ToListAsync();
        serviceResponse.Messages.Add("Successfully retrieved all reflections from every user");

        // Add mapped User, Child, and List<Emotion> entities to each ReflectionChild returned in service response data
        foreach (GetReflectionDto reflection in serviceResponse.Data)
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
      }
      catch (Exception ex)
      {
        serviceResponse.Status = HttpStatusCode.InternalServerError;
        serviceResponse.Messages.Add(ex.Message);
        serviceResponse.Success = false;
      }
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetUserReflectionDto>> GetReflectionById(Guid id)
    {
      ServiceResponse<GetUserReflectionDto> serviceResponse = new();
      serviceResponse.Data = _mapper.Map<GetUserReflectionDto>(await _db.Reflections
        .Include(r => r.Capture)
        .Include(r => r.Comment)
        .Include(r => r.Children)
        .FirstOrDefaultAsync(c => c.Id == id));

      // Add mapped Child and List<Emotion> entities to ReflectionChild returned in service response data
      await RetrieveReflectionChildAndEmotions(serviceResponse.Data.Children);

      await _db.SaveChangesAsync();
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflection"></param>
    /// <returns></returns>
    private async Task RetrieveReflectionChildAndEmotions(ICollection<GetReflectionChildDto> children)
    {
      foreach (var reflectionChild in children)
      {
        // Retrieve child and emotion
        GetChildDto child = _mapper.Map<GetChildDto>(await _db.Children.FirstOrDefaultAsync(c => c.ReflectionChild.Id == reflectionChild.Id));
        ICollection<GetEmotionDto> emotions = _mapper.Map<List<GetEmotionDto>>(await _db.Emotions.Where(e => e.ReflectionChild.Id == reflectionChild.Id).ToListAsync());

        // Assign child and emotion to ReflectionChild entity
        reflectionChild.Emotions = emotions;
        reflectionChild.Child = child;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflection"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> PostReflection(AddReflectionStringDataDto reflection)
    {
      try
      {
        ServiceResponse<Guid> serviceResponse = new();

        // Convert DTO with string capture data and convert into bytes
        AddReflectionByteDataDto reflectionAsBytes = new()
        {
          Capture = new AddCaptureDto() { Data = Encoding.UTF8.GetBytes(reflection.Capture.Data) },
          CreationTime = DateTimeOffset.UtcNow
        };

        // Map reflection and assign User as bearer retrieved from HttpContext via GetUserId()
        Reflection dbReflection = _mapper.Map<Reflection>(reflectionAsBytes);
        dbReflection.User = await _db.Users.FirstOrDefaultAsync(u => u.Id == _authService.GetUserId());

        // Add new reflection using automapper and track changes with EntityEntry object (tracking Id)
        EntityEntry<Reflection> addedReflection = await _db.Reflections.AddAsync(dbReflection);
        await _db.SaveChangesAsync();

        // Retrieve reflection using automapper
        serviceResponse.Data = addedReflection.Entity.Id;
        return serviceResponse;
      }
      catch (Exception ex)
      {
        return new ServiceResponse<Guid>()
        {
          Status = HttpStatusCode.InternalServerError,
          Messages = new List<string>() { ex.Message },
          Success = false
        };
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflectionChild"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> PostReflectionChildEmotion(AddReflectionChildEmotionDto reflectionChild)
    {
      ServiceResponse<Guid> serviceResponse = new();
      try
      {
        // --- Retrieve Reflection and Child
        Reflection reflection = await _db.Reflections.FindAsync(reflectionChild.ReflectionId);
        Child child = await _db.Children.FindAsync(reflectionChild.ChildId);

        // --- Retrieve ReflectionChild for retrieved Reflection and Child.
        ReflectionChild retrievedReflectionChild = await _db.ReflectionChildren
          .Include(r => r.Emotions)
          .FirstOrDefaultAsync(rc => rc.Reflection.Id == reflectionChild.ReflectionId && rc.ChildId == reflectionChild.ChildId);

        // -- Check for duplicate Emotion for this ReflectionChild
        Emotion duplicate = await _db.Emotions.FirstOrDefaultAsync(e => e.Type == reflectionChild.Emotion.Type);
        if (duplicate == null)
        {
          // --- Create Emotion for newly added ReflectionChild
          EntityEntry<Emotion> emotion = await _db.Emotions.AddAsync(new Emotion() { Type = reflectionChild.Emotion.Type, Intensity = reflectionChild.Emotion.Intensity, ReflectionChild = retrievedReflectionChild });
          await _db.SaveChangesAsync();

          // Retrieve reflection using automapper
          serviceResponse.Data = emotion.Entity.Id;
        }
        else
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add("Emotion already exists.");
          serviceResponse.Status = HttpStatusCode.Conflict;
        }
      }
      catch (Exception ex)
      {
        serviceResponse.Status = HttpStatusCode.InternalServerError;
        serviceResponse.Messages = new List<string>() { ex.Message };
        serviceResponse.Success = false;
      }
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflection"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetCommentDto>> UpdateReflectionComment(UpdateReflectionCommentDto reflection)
    {
      ServiceResponse<GetCommentDto> serviceResponse = new();
      try
      {
        // Retrieve comment
        Comment comment = await _db.Comments
          .Include(c => c.Reflection)
          .FirstOrDefaultAsync(c => c.ReflectionId == reflection.ReflectionId);

        // Verify comment is found
        if (comment == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add($"Comment for reflection with id {reflection.ReflectionId} not found.");
          return serviceResponse;
        }

        // Update comment
        comment.Text = reflection.Comment.Text;
        comment.UpdatedTime = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();

        serviceResponse.Data = _mapper.Map<GetCommentDto>(comment);
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Messages.Add(ex.Message);
      }
      return serviceResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<List<GetCaptureDto>>> GetReflectionCaptures()
    {
      return new ServiceResponse<List<GetCaptureDto>> 
      { 
        Data = await _db.Captures
          .Include(c => c.Reflection)
          .Select(c => _mapper.Map<GetCaptureDto>(c))
          .ToListAsync()
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<GetCaptureDto>> GetReflectionCaptureById(Guid id)
    {
      return new ServiceResponse<GetCaptureDto>
      {
        Data = _mapper.Map<GetCaptureDto>(await _db.Captures
          .Include(c => c.Reflection)
          .FirstOrDefaultAsync(c => c.Id == id))
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reflectionChild"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> PostReflectionChild(AddReflectionChildDto reflectionChild)
    {
      ServiceResponse<Guid> serviceResponse = new();
      try
      {
        // Check if reflection already has child being added
        if ((await _db.ReflectionChildren.FirstOrDefaultAsync(rc => rc.ChildId == reflectionChild.ChildId)) != null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Child with id {reflectionChild.ChildId} already added to reflection.");
          serviceResponse.Status = HttpStatusCode.Conflict;
          return serviceResponse;
        }

        // Attempt to retrieve a child and check if found
        Child child = await _db.Children.FirstOrDefaultAsync(c => c.Id == reflectionChild.ChildId);
        if (child == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Child with id {reflectionChild.ChildId} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        // Attempt to retrieve a reflection and check if found
        Reflection reflection = await _db.Reflections.FirstOrDefaultAsync(r => r.Id == reflectionChild.ReflectionId);
        if (reflection == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Reflection with id {reflectionChild.ReflectionId} not found.");
          serviceResponse.Status = HttpStatusCode.NotFound;
          return serviceResponse;
        }

        ReflectionChild newReflectonChild = new() { Child = child, Reflection = reflection };
        EntityEntry<ReflectionChild> addedReflectionChild = await _db.ReflectionChildren.AddAsync(newReflectonChild);
        await _db.SaveChangesAsync();
        if (addedReflectionChild.State != EntityState.Added)
        {
          serviceResponse.Success = false;
          serviceResponse.Messages.Add($"Failed to add child to reflection.");
          serviceResponse.Status = HttpStatusCode.InternalServerError;
          return serviceResponse;
        }

        serviceResponse.Messages.Add($"Successfully added child with id {child.Id} to reflection with id {reflection.Id}.");
        serviceResponse.Data = addedReflectionChild.Entity.Id;
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
    /// <param name="reflectionId"></param>
    /// <param name="childId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<Guid>> DeleteReflectionChild(Guid reflectionId, Guid childId)
    {
      ServiceResponse<Guid> serviceResponse = new();
      try
      {
        ReflectionChild reflectionChild = await _db.ReflectionChildren
          .Include(rc => rc.Reflection)
          .FirstOrDefaultAsync(rc => rc.ChildId == childId && rc.Reflection.Id == reflectionId);
        if (reflectionChild == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Status = HttpStatusCode.NotFound;
          serviceResponse.Messages.Add($"Reflection with id {reflectionId} has no child with id {childId}");
          return serviceResponse;
        }

        _db.ReflectionChildren.Remove(reflectionChild);
        await _db.SaveChangesAsync();

        serviceResponse.Messages.Add($"Successfully removed child with id {childId} from reflection with id {reflectionId}");
        serviceResponse.Data = reflectionChild.Id;
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
