using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Abstractions;

public interface IToDomainMapper<in TPersistence, TDomain>
{
    Result<TDomain, string> ToDomain(TPersistence persistence);
}

public interface IToPersistenceMapper<in TDomain, out TPersistence>
{
    TPersistence ToPersistence(TDomain domain);
}

public interface IApplyToRecordMapper<in TDomain, in TPersistence>
{
    void ApplyToRecord(TDomain domain, TPersistence record);
}

public interface IAggregateMapper<TPersistence, TDomain> : 
    IToDomainMapper<TPersistence, TDomain>,
    IToPersistenceMapper<TDomain, TPersistence>,
    IApplyToRecordMapper<TDomain, TPersistence>;
