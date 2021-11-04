using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using PhenomenologicalStudy.API.Configuration;
using PhenomenologicalStudy.API.Models;

namespace PhenomenologicalStudy.API.Data
{
  public static class DbInitializer
  {
    /// <summary>
    /// Needed to access secrets stored locally in a secrets.js file
    /// </summary>
    public static Secrets Secrets { get; set; } // Use application settings to access secrets from environment configuration 
    public static IConfiguration Configuration { get; set; } // Access secrets from an IConfiguration service with key vault configured

    public static async Task<int> SeedUsersAndRoles(IServiceProvider serviceProvider)
    {
      // create the database if it doesn't exist
      PhenomenologicalStudyContext context = serviceProvider.GetRequiredService<PhenomenologicalStudyContext>();
      context.Database.Migrate();

      RoleManager<Role> roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
      UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();

      // Check if roles already exist and exit if there are
      if (roleManager.Roles.Any())
        return 1;  // should log an error message here

      // Seed roles
      int result = await SeedRoles(roleManager);
      if (result != 0)
        return 2;  // should log an error message here

      // Check if users already exist and exit if there are
      if (userManager.Users.Count() > 0)
        return 3;  // should log an error message here

      // Seed users
      result = await SeedUsers(userManager);
      if (result != 0)
        return 4;  // should log an error message here

      return 0;
    }

    private static async Task<int> SeedRoles(RoleManager<Role> roleManager)
    {
      // Create Admin Role
      var result = await roleManager.CreateAsync(new Role("Admin"));
      if (!result.Succeeded)
        return 1;  // should log an error message here

      // Create Participant Role
      result = await roleManager.CreateAsync(new Role("Participant"));
      if (!result.Succeeded)
        return 2;  // should log an error message here

      return 0;
    }

    private static async Task<int> SeedUsers(UserManager<User> userManager)
    {
      // Create Admin User
      var adminUser = new User
      {
        UserName = "demo.admin@example.ca",
        Email = "demo.admin@example.ca",
        FirstName = "Demo",
        LastName = "Admin",
        EmailConfirmed = true
      };
      //var result = await userManager.CreateAsync(adminUser, AppSecrets.DemoAdminPassword);
      var result = await userManager.CreateAsync(adminUser, Configuration["DemoAdminPassword"]);
      if (!result.Succeeded)
        return 1;  // should log an error message here

      // Assign user to Admin role
      result = await userManager.AddToRoleAsync(adminUser, "Admin");
      if (!result.Succeeded)
        return 2;  // should log an error message here

      // Add claim for a permission with a value of either "grant" or "deny"
      result = await userManager.AddClaimAsync(adminUser, new Claim("examplePermission", "grant"));
      if (!result.Succeeded)
        return 7;  // should log an error message here

      // Create Participant User
      var participantUser = new User
      {
        UserName = "demo.participant@example.ca",
        Email = "demo.participant@example.ca",
        FirstName = "Demo",
        LastName = "Participant",
        EmailConfirmed = true
      };
      //result = await userManager.CreateAsync(managerUser, AppSecrets.DemoParticipantPassword);
      result = await userManager.CreateAsync(participantUser, Configuration["DemoParticipantPassword"]);
      if (!result.Succeeded)
        return 3;  // should log an error message here

      // Assign user to Manager role
      result = await userManager.AddToRoleAsync(participantUser, "Participant");
      if (!result.Succeeded)
        return 4;  // should log an error message 

      return 0; // Log seed success message
    }
  }
}
