using PhenomenologicalStudy.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Authentication.Request
{
  public class ConfirmEmailRequest
  {
    public string UserId { get; set; }

    public string Code { get; set; }
  }
}
