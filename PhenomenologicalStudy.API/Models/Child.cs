using Microsoft.AspNetCore.Identity;
using PhenomenologicalStudy.API.Models.ManyToMany;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Child
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public char Gender { get; set; }
    public User User { get; set; }  // User 1...* Child
    public Reflection Reflection { get; set; }  // Reflection 0...* Child
    public Guid? ReflectionId { get; set; } // Made nullable to produce 0...* relationship
    public ReflectionChild ReflectionChild { get; set; }  // ReflectionChild 1...1 Child 
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
  }
}
