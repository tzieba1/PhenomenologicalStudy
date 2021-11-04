using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Configuration
{
  /// <summary>
  /// Do not inherit from any entity classes or interfaces, and do not connect to app context or the database.
  /// </summary>
  public class Secrets
  {
    public string DemoAdminPassword { get; set; }
    public string DemoParticipantPassword { get; set; }
  }
}
