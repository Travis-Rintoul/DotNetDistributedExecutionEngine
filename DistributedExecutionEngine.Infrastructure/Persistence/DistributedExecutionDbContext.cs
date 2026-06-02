using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs;
using DistributedExecutionEngine.Infrastructure.Persistence.JobTypes;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Persistence;

public sealed class DistributedExecutionDbContext(DbContextOptions<DistributedExecutionDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<JobRecord> Jobs { get; set; }
    public DbSet<WorkerRecord> Workers { get; set; }
    public DbSet<JobTypeRecord> JobTypes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DistributedExecutionDbContext).Assembly);
    }
}

