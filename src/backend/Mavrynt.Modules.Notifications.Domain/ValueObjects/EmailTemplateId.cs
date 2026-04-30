using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Domain.Errors;

namespace Mavrynt.Modules.Notifications.Domain.ValueObjects;

public sealed class EmailTemplateId : ValueObject
{
    public Guid Value { get; }

    private EmailTemplateId(Guid value) => Value = value;

    public static Result<EmailTemplateId> New()
    {
        var id = Guid.NewGuid();
        return id == Guid.Empty
            ? NotificationsErrors.InvalidEmailTemplateId
            : new EmailTemplateId(id);
    }

    public static Result<EmailTemplateId> From(Guid value)
    {
        return value == Guid.Empty
            ? NotificationsErrors.InvalidEmailTemplateId
            : new EmailTemplateId(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
