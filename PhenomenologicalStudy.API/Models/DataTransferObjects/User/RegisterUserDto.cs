﻿using System.ComponentModel.DataAnnotations;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.User
{
  public class RegisterUserDto
  {
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}
