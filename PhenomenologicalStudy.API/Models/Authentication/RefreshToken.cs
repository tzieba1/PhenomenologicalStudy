﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.Authentication
{
  public class RefreshToken
  {
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public string Token { get; set; }

    public Guid JwtId { get; set; }

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset ExpiryDate { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
  }
}
