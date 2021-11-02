using Microsoft.AspNetCore.Identity;
using System;
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

    public int? ImageId { get; set; }
  }
}
