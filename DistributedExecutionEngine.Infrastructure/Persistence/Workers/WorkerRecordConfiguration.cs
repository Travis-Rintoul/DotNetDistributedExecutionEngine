using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public class WorkerRecordConfiguration: IEntityTypeConfiguration<WorkerRecord>
{
    public void Configure(EntityTypeBuilder<WorkerRecord> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.WorkerId)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasIndex(x => x.WorkerId)
            .IsUnique();
        
        builder.Property(x => x.Status)
            .HasConversion<int>();
    }
}