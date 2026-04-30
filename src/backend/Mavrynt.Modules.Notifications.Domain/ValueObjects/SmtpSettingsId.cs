using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Domain.Errors;

namespace Mavrynt.Modules.Notifications.Domain.ValueObjects;

public sealed class SmtpSettingsId : ValueObject
{
    public Guid Value { get; }

    private SmtpSettingsId(Guid value) => Value = value;

    public static Result<SmtpSettingsId> New()
    {
        var id = Guid.NewGuid();
        return id == Guid.Empty
            ? NotificationsErrors.InvalidSmtpSettingsId
            : new SmtpSettingsId(id);
    }

    public static Result<SmtpSettingsId> From(Guid value)
    {
        return value == Guid.Empty
            ? NotificationsErrors.InvalidSmtpSettingsId
            : new SmtpSettingsId(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
