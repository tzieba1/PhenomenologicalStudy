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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.IO;
using PhenomenologicalStudy.API.Models.ManyToMany;
using ReflectionAPI.Models;

namespace PhenomenologicalStudy.API.Data
{
  public static class DbInitializer
  {
    /// <summary>
    /// Needed to access secrets stored locally in a secrets.js file
    /// </summary>
    public static DbInitializerConfiguration Configuration { get; set; } // Use application settings to access secrets from environment configuration 
    //public static IConfiguration Configuration { get; set; } // Access secrets from an IConfiguration service with key vault configured

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
      if (userManager.Users.Any())
        return 3;  // should log an error message here

      // Seed users
      result = await SeedUsers(userManager, context);
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

    private static async Task<int> SeedUsers(UserManager<User> userManager, PhenomenologicalStudyContext context)
    {
      // --- Create Admin User
      User adminUser = new()
      {
        UserName = "demo.admin@example.com",
        Email = "demo.admin@example.com",
        FirstName = "Demo",
        LastName = "Admin",
        EmailConfirmed = true
      };
      IdentityResult result = await userManager.CreateAsync(adminUser, Configuration.DemoAdminPassword);
      //var result = await userManager.CreateAsync(adminUser, Configuration["DemoAdminPassword"]);
      if (!result.Succeeded)
        return 1;  // should log an error message here

      // Retrieve created admin for more demo data to be added (related to them)
      User createdAdmin = await userManager.FindByEmailAsync(adminUser.Email);

      // Assign user to Admin role in the database
      result = await userManager.AddToRoleAsync(createdAdmin, "Admin");
      if (!result.Succeeded)
        return 2;  // should log an error message here

      // Add "Admin" claim to roles
      result = await userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "Admin"));
      if (!result.Succeeded)
        return 3;  // should log an error message here

      // Add claim for a permission with a value of either "grant" or "deny"
      //result = await userManager.AddClaimAsync(adminUser, new Claim("examplePermission", "grant"));

      //IList<string> roles = await userManager.GetRolesAsync(createdAdmin);  // Retrieve roles

      // --- Create Participant User
      User participantUser = new()
      {
        UserName = "demo.participant@example.com",
        Email = "demo.participant@example.com",
        FirstName = "Demo",
        LastName = "Participant",
        EmailConfirmed = true
      };

      result = await userManager.CreateAsync(participantUser, Configuration.DemoParticipantPassword);
      //result = await userManager.CreateAsync(participantUser, Configuration["DemoParticipantPassword"]);
      if (!result.Succeeded)
        return 3;  // should log an error message here

      // Retrieve created participant for more demo data to be added (related to them)
      User createdParticipant = await userManager.FindByEmailAsync(participantUser.Email);

      // Assign user to Particiapnt role
      result = await userManager.AddToRoleAsync(createdParticipant, "Participant");
      if (!result.Succeeded)
        return 4;  // should log an error message

      // Add "Participant" claim to roles
      result = await userManager.AddClaimAsync(createdParticipant, new Claim(ClaimTypes.Role, "Participant"));
      if (!result.Succeeded)
        return 5;  // should log an error message here

      // --- Create Child for demo participant
      Child child = new()
      {
        User = createdParticipant,
        FirstName = "Demo",
        LastName = "Child",
        DateOfBirth = new DateTimeOffset(new DateTime(2015, 04, 15)),
        Gender = 'M'
      };
      EntityEntry<Child> addedChild = await context.Children.AddAsync(child); // Add newly related child to the database

      // --- Create Reflection for demo participant
      Reflection reflection = new() { User = createdParticipant, UpdatedTime = DateTimeOffset.UtcNow };
      EntityEntry<Reflection> addedReflection = await context.Reflections.AddAsync(reflection);

      // --- Add Reflection to Child (FK -> Child.ReflectionId)
      addedChild.Entity.Reflection = addedReflection.Entity;

      // --- Create ReflectionChild for newly created Reflection and Child.
      EntityEntry<ReflectionChild> addedReflectionChild =
        await context.ReflectionChildren.AddAsync(new ReflectionChild() { Child = addedChild.Entity, Reflection = addedReflection.Entity });

      // --- Create Capture for demo reflection
      Capture capture = new() { Reflection = reflection, Data = ConvertImageToByteArray("./Images/demo1.jpg") };
      await context.Captures.AddAsync(capture);

      // --- Create Comment for demo reflection
      Comment comment = new() { Text = "Demo comment.", Reflection = reflection, UpdatedTime = DateTimeOffset.UtcNow };
      await context.Comments.AddAsync(comment);

      //ReflectionChild joinReflectionChild = await context.FindAsync(addedReflectionChild.Entity.Id);
      // --- Create Emotion list for demo child and demo reflection that will relate to a single ReflectionChild
      await context.Emotions.AddAsync(new Emotion() { Type = EmotionType.Overwhelmed, Intensity = 3, ReflectionChild = addedReflectionChild.Entity });
      await context.Emotions.AddAsync(new Emotion() { Type = EmotionType.Frustrated, Intensity = 6, ReflectionChild = addedReflectionChild.Entity });

      await context.SaveChangesAsync(); // Save database changes

      return 0; // Log seed success message
    }

    private static byte[] ConvertImageToByteArray(string imagePath)
    {
      byte[] imageByteArray = null;
      FileStream fileStream = new(imagePath, FileMode.Open, FileAccess.Read);
      using (BinaryReader reader = new(fileStream))
      {
        imageByteArray = new byte[reader.BaseStream.Length];
        for (int i = 0; i < reader.BaseStream.Length; i++)
          imageByteArray[i] = reader.ReadByte();
      }
      return imageByteArray;
    }
  }
}
