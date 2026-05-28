using DistributedExecutionEngine.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistributedExecutionEngine.Infrastructure.Persistence.JobType;

public class JobTypeRecordConfiguration : IEntityTypeConfiguration<JobTypeRecord>
{
    public void Configure(EntityTypeBuilder<JobTypeRecord> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Key)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Key)
            .IsUnique();
    }
}