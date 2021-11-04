using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Authorization.Requirements
{
  // Configure a requirement to add logic to static claims by extending a base interface
  public class ExamplePermissionRequirement : IAuthorizationRequirement
  {
    // Property to be used as an additional requirement for a claim
    public string Result { get; private set; }

    // Construct the requirement by initializing required properties
    public ExamplePermissionRequirement(string result)
    {
      Result = result;
    }
  }
}
