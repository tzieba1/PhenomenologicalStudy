using Microsoft.EntityFrameworkCore;
using PhenomenologicalStudy.API.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Data
{
  public interface IPhenomenologicalStudyContext : IDisposable
  {
    public DbSet<ErrorMessage> ErrorMessages { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  }
}
