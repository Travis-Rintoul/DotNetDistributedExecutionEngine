using System.Data;
using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Features.Workers.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Supervisor;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NpgsqlTypes;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public class WorkerPoolStore(
    IClock clock,
    IWorkerMapper workerMapper,
    DistributedExecutionDbContext context
) : IWorkerPoolStore
{
    public async Task<Result<IReadOnlyList<WorkerId>, string>> ClaimPendingWorkersForStartup(SupervisorId supervisorId, int limit, CancellationToken cancellationToken)
    {
        const string sql =
            """
            UPDATE "Workers" w
            SET 
                "StatusCode" = @startingStatus,
                "SupervisionLeasedBy" = @supervisorId,
                "SupervisionLeasedUtc" = @nowUtc,
                "SupervisionLeaseExpiresUtc" = @leaseExpiresUtc,
                "LeaseStatusCode" = @leasedStatus
            WHERE w."Id" IN (
                SELECT x."Id"
                FROM "Workers" x
                WHERE x."StatusCode" = @pendingStatus
                AND x."LeaseStatusCode" = @availableStatus
                ORDER BY x."Id"
                FOR UPDATE SKIP LOCKED
                LIMIT @limit
            )
            RETURNING w."WorkerId";
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

        command.Parameters.Add(new NpgsqlParameter<int>("supervisorId", supervisorId.Value)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset>("nowUtc", nowUtc)
        {
            NpgsqlDbType = NpgsqlDbType.TimestampTz
        });

        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset>("leaseExpiresUtc", leaseExpiresUtc)
        {
            NpgsqlDbType = NpgsqlDbType.TimestampTz
        });

        command.Parameters.Add(new NpgsqlParameter<int>("pendingStatus", (int)WorkerStatusCode.Pending)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        command.Parameters.Add(new NpgsqlParameter<int>("startingStatus", (int)WorkerStatusCode.Starting)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        command.Parameters.Add(new NpgsqlParameter<int>("leasedStatus", (int)WorkerLeaseStatusCode.Leased)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        command.Parameters.Add(new NpgsqlParameter<int>("availableStatus", (int)WorkerLeaseStatusCode.Available)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        command.Parameters.Add(new NpgsqlParameter<int>("limit", limit)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });
            
        var workerIds = new List<WorkerId>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var workerId = reader.GetGuid(0);
            workerIds.Add(WorkerId.From(workerId));
        }

        return Result.Success<IReadOnlyList<WorkerId>, string>(workerIds);
    }

    public async Task<Result<IReadOnlyList<WorkerId>, string>> ClaimWorkersForSupervision(SupervisorId supervisorId, int limit, CancellationToken cancellationToken)
    {
        const string sql = 
            """
                UPDATE "Workers" w
                SET 
                    "SupervisionLeasedBy" = @supervisorId,
                    "SupervisionLeasedUtc" = @nowUtc,
                    "SupervisionLeaseExpiresUtc" = @leaseExpiresUtc
                WHERE w."Id" IN (
                    SELECT x."Id"
                    FROM "Workers" x
                    WHERE x."StatusCode" IN (@startingStatus, @runningStatus)
                    AND 
                    (
                         x."SupervisionLeaseExpiresUtc" <= @nowUtc
                         OR x."SupervisionLeasedBy" = @supervisorId
                         OR x."SupervisionLeasedUtc" IS NULL
                    )
                    ORDER BY x."Id"
                    FOR UPDATE SKIP LOCKED
                    LIMIT @limit
                )
                RETURNING w."WorkerId"
             """;
        
        var connection = context.Database.GetDbConnection();

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        
        command.Parameters.Add(new NpgsqlParameter<int>("supervisorId", supervisorId.Value)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset>("nowUtc", clock.UtcNow)
        {
            NpgsqlDbType = NpgsqlDbType.TimestampTz
        });

        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset>("leaseExpiresUtc", clock.UtcNow.AddSeconds(90))
        {
            NpgsqlDbType = NpgsqlDbType.TimestampTz
        });

        command.Parameters.Add(new NpgsqlParameter<int>("startingStatus", (int)WorkerStatusCode.Starting)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        command.Parameters.Add(new NpgsqlParameter<int>("runningStatus", (int)WorkerStatusCode.Running)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        command.Parameters.Add(new NpgsqlParameter<int>("limit", limit)
        {
            NpgsqlDbType = NpgsqlDbType.Integer
        });

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }
        
        var workerIds = new List<WorkerId>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var workerId = reader.GetGuid(0);
            workerIds.Add(WorkerId.From(workerId));
        }

        return Result.Success<IReadOnlyList<WorkerId>, string>(workerIds);
    }

    public async Task<Result<Option<WorkerId>, string>> ReconcileAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var pendingJobCount = await context.Jobs
                .CountAsync(x => x.StatusCode == JobStatusCode.Pending, cancellationToken: cancellationToken);

            var activeWorkerCount = await context.Workers
                .CountAsync(x => x.StatusCode == WorkerStatusCode.Pending || x.StatusCode == WorkerStatusCode.Running || x.StatusCode == WorkerStatusCode.Starting,cancellationToken: cancellationToken);

            if (pendingJobCount == 0 || activeWorkerCount >= 1 || activeWorkerCount >= pendingJobCount)
            {
                await transaction.CommitAsync(cancellationToken);
                return Result<Option<WorkerId>, string>.Success(Option<WorkerId>.None);
            }

            var worker = workerMapper.ToPersistence(Worker.Create());
            
            context.Workers.Add(worker);
            
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            return Result.Success<Option<WorkerId>, string>(Option<WorkerId>.Some(new WorkerId(worker.WorkerId)));

        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<Option<WorkerId>, string>.Failure($"Failed to reconcile worker pool {e.Message}");
        }
    }
}