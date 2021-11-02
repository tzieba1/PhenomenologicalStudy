using Microsoft.EntityFrameworkCore;
using PhenomenologicalStudy.API.Models;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PhenomenologicalStudy.API.Models.Authentication;

namespace PhenomenologicalStudy.API.Data
{
  public class PSDbContext : IdentityDbContext<User, Role, Guid>
  {
    public PSDbContext(DbContextOptions<PSDbContext> options) : base(options) { }

    public virtual DbSet<Reflection> Reflections { get; set; }
    public virtual DbSet<Child> Children { get; set; }
    public virtual DbSet<Emotion> Emotions { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Image> Images { get; set; }
    public virtual DbSet<UserReflection> UserReflections { get; set; }
    public virtual DbSet<UserChild> UserChildren { get; set; }
    public virtual DbSet<ChildEmotion> ChildEmotions { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
  }
}
