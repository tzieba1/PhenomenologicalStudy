using Microsoft.EntityFrameworkCore;
using PhenomenologicalStudy.API.Models;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PhenomenologicalStudy.API.Data
{
  public class UserDbContext : IdentityDbContext<User, Role, Guid>
  {
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {

    }

    public DbSet<Reflection> Reflections { get; set; }
    public DbSet<Child> Children { get; set; }
    public DbSet<Emotion> Emotions { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<UserReflection> UserReflections { get; set; }
    public DbSet<UserChild> UserChildren { get; set; }
    public DbSet<ChildEmotion> ChildEmotions { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
  }
}
