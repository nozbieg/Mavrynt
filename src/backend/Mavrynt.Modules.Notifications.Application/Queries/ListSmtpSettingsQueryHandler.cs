using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Application.Mapping;
using Mavrynt.Modules.Notifications.Domain.Repositories;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed class ListSmtpSettingsQueryHandler : IQueryHandler<ListSmtpSettingsQuery, IReadOnlyList<SmtpSettingsDto>>
{
    private readonly ISmtpSettingsRepository _repository;

    public ListSmtpSettingsQueryHandler(ISmtpSettingsRepository repository)
        => _repository = repository;

    public async Task<Result<IReadOnlyList<SmtpSettingsDto>>> HandleAsync(
        ListSmtpSettingsQuery query,
        CancellationToken cancellationToken = default)
    {
        var settings = await _repository.ListAsync(cancellationToken);
        return Result.Success<IReadOnlyList<SmtpSettingsDto>>(
            settings.Select(s => s.ToDto()).ToList().AsReadOnly());
    }
}
