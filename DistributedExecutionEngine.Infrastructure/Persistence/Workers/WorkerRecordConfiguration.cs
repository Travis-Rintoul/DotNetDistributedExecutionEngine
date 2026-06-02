using DistributedExecutionEngine.Application.Features.Workers.Supervision;
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

        builder.Property(x => x.SupervisorId)
            .HasConversion<int?>(
                supervisorId => supervisorId.HasValue
                    ? supervisorId.Value.Value
                    : null,
                value => value.HasValue
                    ? new SupervisorId(value.Value)
                    : null)
            .HasColumnType("integer")
            .ValueGeneratedNever();

        builder.HasIndex(x => x.WorkerId)
            .IsUnique();
        
        builder.Property(x => x.Status)
            .HasConversion<int>();
    }
}