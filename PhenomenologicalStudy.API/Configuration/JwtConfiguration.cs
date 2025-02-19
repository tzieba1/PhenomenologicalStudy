﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Configuration
{
  public class JwtConfiguration
  {
    public JwtConfiguration() { }

    public string ValidAudience { get; set; }
    public string ValidIssuer { get; set; }
    public string PrivateKey { get; set; }
  }
}
