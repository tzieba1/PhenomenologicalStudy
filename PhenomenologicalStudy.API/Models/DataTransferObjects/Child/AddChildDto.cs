using System;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Child
{
  public class AddChildDto
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public char Gender { get; set; }
  }
}
