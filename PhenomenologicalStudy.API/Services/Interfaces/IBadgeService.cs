using PhenomenologicalStudy.API.Models.DataTransferObjects;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Badge;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Services.Interfaces
{
  public interface IBadgeService
  {
    /// <summary>
    /// Represents BadgesController endpoint 'DELETE: /api/Badges/{id}' defined in BadgeService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetBadgeDto>> DeleteBadgeById(Guid id);

    /// <summary>
    /// Represents BadgesController endpoint 'POST: /api/Badges?userId={Guid?}' defined in BadgeService.
    /// </summary>
    /// <param name="Badge"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ServiceResponse<Guid>> PostBadge(AddBadgeDto Badge, Guid? userId);

    /// <summary>
    /// Represents BadgesController endpoint 'PUT: /api/Badges' defined in BadgeService.
    /// </summary>
    /// <param name="badge"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetBadgeDto>> PutBadge(UpdateBadgeDto badge);

    /// <summary>
    /// Represents BadgesController endpoint 'GET: /api/Badges/{id}' defined in BadgeService.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceResponse<GetBadgeDto>> GetBadgeById(Guid id);

    /// <summary>
    /// Represents BadgesController endpoint 'GET: /api/Badges' defined in BadgeService.
    /// </summary>
    /// <returns></returns>
    Task<ServiceResponse<List<GetBadgeDto>>> GetBadges();
  }
}
