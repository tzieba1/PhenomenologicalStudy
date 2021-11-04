using Microsoft.AspNetCore.Authorization;
using PhenomenologicalStudy.API.Authorization.Requirements;
using PhenomenologicalStudy.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Authorization.Handlers
{
  // Handles an authorization requirement
  public class ExamplePermissionRequirementHandler : AuthorizationHandler<ExamplePermissionRequirement>
  {
    // Use handler context to verify an authorization requirement
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExamplePermissionRequirement requirement)
    {
      // Check for a ClaimType before accessing the value to ensure user has claim for the permission
      if (!context.User.HasClaim(c => c.Type == "examplePermission"))
      {
        //TODO: Log that user has no claims to examplePermission
        return Task.CompletedTask;
      }

      // Verify the claim's value (specified when adding that claim for the user found) with any custom logic
      string permissionResult = context.User.FindFirst(c => c.Type == "examplePermission").Value;
      if (permissionResult.Equals(requirement.Result))
      {
        //TODO: Log that user has claims to examplePermission and value is correct (grant)
        context.Succeed(requirement);
      }
      else 
      {
        //TODO: Log that user has claim to examplePermission, but value is not correct (deny)
      }

      return Task.CompletedTask;
    }
  }
}