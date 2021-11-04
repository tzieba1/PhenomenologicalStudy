﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class Role : IdentityRole<Guid>
  {
    public Role() : base() { }
    public Role(string roleName) : base(roleName) { }
  }
}
