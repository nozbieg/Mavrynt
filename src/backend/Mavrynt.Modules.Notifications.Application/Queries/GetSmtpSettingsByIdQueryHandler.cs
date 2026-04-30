using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Application.Mapping;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed class GetSmtpSettingsByIdQueryHandler : IQueryHandler<GetSmtpSettingsByIdQuery, SmtpSettingsDto>
{
    private readonly ISmtpSettingsRepository _repository;

    public GetSmtpSettingsByIdQueryHandler(ISmtpSettingsRepository repository)
        => _repository = repository;

    public async Task<Result<SmtpSettingsDto>> HandleAsync(
        GetSmtpSettingsByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var idResult = SmtpSettingsId.From(query.Id);
        if (idResult.IsFailure) return idResult.Error;

        var settings = await _repository.GetByIdAsync(idResult.Value, cancellationToken);
        if (settings is null) return NotificationsErrors.SmtpSettingsNotFound;

        return settings.ToDto();
    }
}
