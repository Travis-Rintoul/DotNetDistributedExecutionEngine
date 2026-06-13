using System.Data;
using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Features.Jobs.Leasing;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NpgsqlTypes;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobLeasing;

public class JobLeaseStore(IClock clock, DistributedExecutionDbContext context) : IJobLeaseStore
{
    public async Task<Option<JobWorkerLease>> TryLeaseNextJobAsync(WorkerId workerId, CancellationToken cancellationToken)
    {
        const string sql = 
            """
            UPDATE "Jobs" j
            SET
                "LeasedUtc" = @nowUtc,
                "LeastStatusCode" = @leasedStatus,
                "LeaseExpirationUtc" = @leaseExpirationUtc,
                "AssignedWorkerId" = @assignedWorkerId
            WHERE j."Id" = (
                SELECT x."Id"
                FROM "Jobs" x
                WHERE x."StatusCode" IN (@pendingStatus, @failedStatus)
                AND 
                (
                    x."LeaseExpirationUtc" <= @nowUtc
                    OR x."AssignedWorkerId" = @assignedWorkerId
                    OR x."LeasedUtc" IS NULL
                )
                ORDER BY x."Id"
                FOR UPDATE SKIP LOCKED
                LIMIT 1
            )
            RETURNING 
                j."Id",
                j."WorkerId",
                j."LeasedUtc",
                j."LeaseExpirationUtc";
            """;
        
        var nowUtc = clock.UtcNow;
        var leaseExpiresUtc = nowUtc.AddSeconds(90);
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        
        var currentTransaction = context.Database.CurrentTransaction;
        if (currentTransaction is not null)
            command.Transaction = currentTransaction.GetDbTransaction();
        
        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset>("nowUtc", nowUtc)
        {
            NpgsqlDbType = NpgsqlDbType.TimestampTz
        });
        
        command.Parameters.Add(new NpgsqlParameter<int>("leasedStatus", (int)JobLeaseStatusCode.Leased)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });
        
        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset>("leasedStatus", leaseExpiresUtc)
        {
            NpgsqlDbType = NpgsqlDbType.TimestampTz
        });
        
        command.Parameters.Add(new NpgsqlParameter<Guid>("assignedWorkerId", workerId.Value)
        {
            NpgsqlDbType = NpgsqlDbType.Uuid
        });
        
        command.Parameters.Add(new NpgsqlParameter<int>("pendingStatus", (int)JobStatusCode.Pending)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });
        
        command.Parameters.Add(new NpgsqlParameter<int>("failedStatus", (int)JobStatusCode.Failed)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return Option<JobWorkerLease>.None;

        return Option.Some(new JobWorkerLease
        {
            JobId = JobId.From(reader.GetGuid(0)),
            WorkerId = WorkerId.From(reader.GetGuid(1)),
            LeasedAtUtc = reader.GetDateTime(2),
            ExpiresAtUtc =  reader.GetDateTime(3),
        });
    }
}