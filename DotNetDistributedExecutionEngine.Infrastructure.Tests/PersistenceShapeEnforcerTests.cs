using DistributedExecutionEngine.Infrastructure.Persistence.Mapping;
using FluentAssertions;

namespace DotNetDistributedExecutionEngine.Infrastructure.Tests;

public sealed class PersistenceShapeEnforcerTests
{
    private static readonly TestPersistenceField<TestRecord, string?> Name =
        new("Name", x => x.Name, (x, value) => x.Name = value);

    private static readonly TestPersistenceField<TestRecord, int?> Count =
        new("Count", x => x.Count, (x, value) => x.Count = value);

    private static readonly TestPersistenceField<TestRecord, DateTimeOffset?> Timestamp =
        new("Timestamp", x => x.Timestamp, (x, value) => x.Timestamp = value);

    private static readonly IReadOnlyCollection<IPersistenceShapeField<TestRecord>> AllFields =
    [
        Name,
        Count,
        Timestamp
    ];

    private readonly PersistenceShapeEnforcer<TestRecord> _enforcer = new(AllFields);

    [Fact]
    public void Validate_returns_success_when_required_fields_are_set_and_forbidden_fields_are_default()
    {
        var record = new TestRecord
        {
            Name = "worker-1"
        };

        var shape = Shape(
            required: [Name],
            optional: []);

        var result = _enforcer.Validate(record, shape);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_returns_failure_when_required_field_is_default()
    {
        var record = new TestRecord
        {
            Name = null
        };

        var shape = Shape(
            required: [Name],
            optional: []);

        var result = _enforcer.Validate(record, shape);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Required field 'Name' was not set.");
    }

    [Fact]
    public void Validate_returns_success_when_optional_field_is_set()
    {
        var record = new TestRecord
        {
            Name = "worker-1",
            Count = 5
        };

        var shape = Shape(
            required: [Name],
            optional: [Count]);

        var result = _enforcer.Validate(record, shape);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_returns_success_when_optional_field_is_default()
    {
        var record = new TestRecord
        {
            Name = "worker-1",
            Count = null
        };

        var shape = Shape(
            required: [Name],
            optional: [Count]);

        var result = _enforcer.Validate(record, shape);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_returns_failure_when_forbidden_field_is_set()
    {
        var record = new TestRecord
        {
            Name = "worker-1",
            Count = 5
        };

        var shape = Shape(
            required: [Name],
            optional: []);

        var result = _enforcer.Validate(record, shape);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Forbidden field 'Count' was set.");
    }

    [Fact]
    public void Validate_checks_required_fields_before_forbidden_fields()
    {
        var record = new TestRecord
        {
            Name = null,
            Count = 5
        };

        var shape = Shape(
            required: [Name],
            optional: []);

        var result = _enforcer.Validate(record, shape);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Required field 'Name' was not set.");
    }

    [Fact]
    public void ResetForbidden_resets_only_forbidden_fields()
    {
        var record = new TestRecord
        {
            Name = "worker-1",
            Count = 5,
            Timestamp = DateTimeOffset.UtcNow
        };

        var shape = Shape(
            required: [Name],
            optional: [Timestamp]);

        _enforcer.ResetForbidden(record, shape);

        record.Name.Should().Be("worker-1");
        record.Timestamp.Should().NotBeNull();
        record.Count.Should().BeNull();
    }

    [Fact]
    public void Apply_resets_forbidden_fields_then_validates_required_fields()
    {
        var record = new TestRecord
        {
            Name = "worker-1",
            Count = 5,
            Timestamp = DateTimeOffset.UtcNow
        };

        var shape = Shape(
            required: [Name],
            optional: []);

        var result = _enforcer.Apply(record, shape);

        result.IsSuccess.Should().BeTrue();

        record.Name.Should().Be("worker-1");
        record.Count.Should().BeNull();
        record.Timestamp.Should().BeNull();
    }

    [Fact]
    public void Apply_returns_failure_when_required_field_is_default_after_reset()
    {
        var record = new TestRecord
        {
            Name = null,
            Count = 5
        };

        var shape = Shape(
            required: [Name],
            optional: []);

        var result = _enforcer.Apply(record, shape);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Required field 'Name' was not set.");

        record.Count.Should().BeNull();
    }

    [Fact]
    public void ValidateRequired_returns_success_when_required_fields_are_set()
    {
        var record = new TestRecord
        {
            Name = "worker-1",
            Count = 5
        };

        var shape = Shape(
            required: [Name, Count],
            optional: []);

        var result = _enforcer.ValidateRequired(record, shape);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ValidateRequired_returns_failure_when_required_field_is_default()
    {
        var record = new TestRecord
        {
            Name = "worker-1",
            Count = null
        };

        var shape = Shape(
            required: [Name, Count],
            optional: []);

        var result = _enforcer.ValidateRequired(record, shape);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Required field 'Count' was not set.");
    }

    private static PersistenceShape<TestRecord> Shape(
        IReadOnlyCollection<IPersistenceShapeField<TestRecord>> required,
        IReadOnlyCollection<IPersistenceShapeField<TestRecord>> optional)
    {
        return PersistenceShape<TestRecord>.For(
            all: AllFields,
            required: required,
            optional: optional);
    }

    private sealed class TestRecord
    {
        public string? Name { get; set; }
        public int? Count { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }

    private sealed class TestPersistenceField<TRecord, TValue>(
        string name,
        Func<TRecord, TValue> get,
        Action<TRecord, TValue> set) : IPersistenceShapeField<TRecord>
    {
        public string Name { get; } = name;

        public bool HasValue(TRecord record)
            => !IsDefault(record);

        public bool IsDefault(TRecord record)
            => EqualityComparer<TValue>.Default.Equals(get(record), default);

        public void Reset(TRecord record)
            => set(record, default!);
    }
}