using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Abstractions;

public abstract record MappingError
{
    public sealed record MissingRequiredValue(string FieldName) : MappingError;
    public sealed record InvalidValue(string FieldName, string Reason) : MappingError;
    public sealed record UnsupportedStatus(string Status) : MappingError;
}

public interface IToDomainMapper<in TPersistence, TDomain>
{
    Result<TDomain, MappingError> ToDomain(TPersistence persistence);
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
