using Microsoft.EntityFrameworkCore;
using PhenomenologicalStudy.API.Models;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PhenomenologicalStudy.API.Models.Authentication;

namespace PhenomenologicalStudy.API.Data
{
  public class PhenomenologicalStudyContext : IdentityDbContext<User, Role, Guid>, IPhenomenologicalStudyContext
  {
    public PhenomenologicalStudyContext(DbContextOptions<PhenomenologicalStudyContext> options) : base(options) { }

    public virtual DbSet<Reflection> Reflections { get; set; }
    public virtual DbSet<Child> Children { get; set; }
    public virtual DbSet<Emotion> Emotions { get; set; }
    public virtual DbSet<ChildEmotion> ChildrenEmotions { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Capture> Captures { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<ErrorMessage> ErrorMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<ChildEmotion>().HasKey(t => new { t.ChildId, t.ReflectionId });
    }
  }
}
