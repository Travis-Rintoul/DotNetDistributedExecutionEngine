namespace DistributedExecutionEngine.Infrastructure.Persistence.Mapping;

public sealed class PersistenceShape<TRecord>
{
   public IReadOnlySet<IPersistenceShapeField<TRecord>> Required { get; }
   public IReadOnlySet<IPersistenceShapeField<TRecord>> Optional { get; }
   public IReadOnlySet<IPersistenceShapeField<TRecord>> Forbidden { get; }

   private PersistenceShape(
      IReadOnlySet<IPersistenceShapeField<TRecord>> required,
      IReadOnlySet<IPersistenceShapeField<TRecord>> optional,
      IReadOnlySet<IPersistenceShapeField<TRecord>> forbidden)
   {
      Required = required;
      Optional = optional;
      Forbidden = forbidden;
   }

   public static PersistenceShape<TRecord> For(
      IEnumerable<IPersistenceShapeField<TRecord>> required,
      IEnumerable<IPersistenceShapeField<TRecord>> optional,
      IEnumerable<IPersistenceShapeField<TRecord>> all)
   {
      var requiredSet = required.ToHashSet();
      var optionalSet = optional.ToHashSet();

      var allowed = requiredSet.Concat(optionalSet).ToHashSet();
      var forbiddenSet = all.Except(allowed).ToHashSet();

      return new PersistenceShape<TRecord>(
         requiredSet,
         optionalSet,
         forbiddenSet);
   }
}