using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhenomenologicalStudy.API.Models
{
  public class User : IdentityUser<Guid>
  {
    [Required(ErrorMessage = "First Name is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    public string LastName { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email address is required")]
    public override string Email { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTimeOffset? BirthDate { get; set; }

    public ICollection<Child> Children { get; set; }  // Child *...1 User
    public ICollection<Reflection> Reflections { get; set; }  // Reflection *...1 User
    public ICollection<RefreshToken> RefreshTokens { get; set; }  // RefreshToken *...1 User
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
  }
}
