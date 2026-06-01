using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Features.Workers.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public class WorkerPoolStore(
    DistributedExecutionDbContext context
) : IWorkerPoolStore
{
    public async Task<Result<Option<WorkerId>, string>> ClaimNextPendingWorker(DateTimeOffset nowUtc, SupervisorId supervisorId, CancellationToken cancellationToken)
    {
        try
        {
            FormattableString query = 
                $"""
                     UPDATE "Workers"
                     SET
                         "ClaimedUtc" = {nowUtc},
                         "SupervisorId" = {supervisorId}
                     WHERE "Id" = (
                         SELECT "Id"
                         FROM "Workers"
                         WHERE "Status" = {(int)WorkerStatusCode.Pending}
                         ORDER BY "CreatedUtc", "Id"
                         FOR UPDATE SKIP LOCKED
                         LIMIT 1
                     )
                     RETURN "WorkerId"
                 """;

            var workerId = await context.Database
                .SqlQuery<Guid>(query)
                .SingleOrDefaultAsync(cancellationToken);
            
            var result = (workerId == Guid.Empty)
                ? Option<WorkerId>.None
                : Option<WorkerId>.Some(new WorkerId(workerId));

            return Result<Option<WorkerId>, string>.Success(result);
        }
        catch (Exception e)
        {
            return Result<Option<WorkerId>, string>.Failure(e.Message);
        }
    }
    
    public async Task<Result<Option<WorkerId>, string>> ReconcileAsync(DateTimeOffset nowUtc, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var pendingJobCount = await context.Jobs
                .CountAsync(x => x.Status == JobStatusCode.Pending, cancellationToken: cancellationToken);

            var activeWorkerCount = await context.Workers
                .CountAsync(x => x.Status == WorkerStatusCode.Running, cancellationToken: cancellationToken);

            if (pendingJobCount == 0 || activeWorkerCount >= 1)
            {
                await transaction.CommitAsync(cancellationToken);
                return Result<Option<WorkerId>, string>.Success(Option<WorkerId>.None);
            }

            var worker = WorkerMapper.ToPersistence(Worker.Create());
            
            context.Workers.Add(worker);
            
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            return Result.Success<Option<WorkerId>, string>(Option<WorkerId>.Some(new WorkerId(worker.WorkerId)));

        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<Option<WorkerId>, string>.Failure("Failed to reconcile worker pool.");
        }
    }
}