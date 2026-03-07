using DistributedExecutionEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Configuration;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Guid)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(x => x.Guid).IsUnique();
        
        builder.Property(x => x.PayloadJson)
            .HasColumnType("jsonb");

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.JobType)
            .IsRequired();
    }
}