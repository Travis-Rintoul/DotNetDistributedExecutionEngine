using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistributedExecutionEngine.Infrastructure.Persistence.JobTypes;

public class JobTypeRecordConfiguration : IEntityTypeConfiguration<JobTypeRecord>
{
    public void Configure(EntityTypeBuilder<JobTypeRecord> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Code);
        
        builder.Property(x => x.Code)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}