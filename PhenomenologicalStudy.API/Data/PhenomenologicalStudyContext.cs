using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.ManyToMany;

namespace PhenomenologicalStudy.API.Data
{
  public class PhenomenologicalStudyContext : IdentityDbContext<User, Role, Guid>
  {
    public PhenomenologicalStudyContext(DbContextOptions<PhenomenologicalStudyContext> options) : base(options) { }

    public virtual DbSet<Badge> Badges { get; set; }
    public virtual DbSet<Capture> Captures { get; set; }
    public virtual DbSet<Child> Children { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Emotion> Emotions { get; set; }
    public virtual DbSet<ErrorMessage> ErrorMessages { get; set; }
    public virtual DbSet<PRFQuestionnaire> Questionnaires { get; set; }
    public virtual DbSet<Reflection> Reflections { get; set; }
    public virtual DbSet<ReflectionChild> ReflectionChildren { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<Badge>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<Capture>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<Child>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<Comment>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<Emotion>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<ErrorMessage>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<PRFQuestionnaire>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<Reflection>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<ReflectionChild>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
      builder.Entity<RefreshToken>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
    }
  }
}
