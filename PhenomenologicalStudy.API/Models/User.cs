using Microsoft.AspNetCore.Identity;
using PhenomenologicalStudy.API.Models.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public byte[] ProfilePicture { get; set; }

    public ICollection<Child> Children { get; set; }

    public ICollection<Reflection> Reflections { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; }
  }
}
