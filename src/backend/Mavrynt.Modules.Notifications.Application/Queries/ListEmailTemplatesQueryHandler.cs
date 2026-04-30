using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Application.Mapping;
using Mavrynt.Modules.Notifications.Domain.Repositories;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed class ListEmailTemplatesQueryHandler : IQueryHandler<ListEmailTemplatesQuery, IReadOnlyList<EmailTemplateDto>>
{
    private readonly IEmailTemplateRepository _repository;

    public ListEmailTemplatesQueryHandler(IEmailTemplateRepository repository)
        => _repository = repository;

    public async Task<Result<IReadOnlyList<EmailTemplateDto>>> HandleAsync(
        ListEmailTemplatesQuery query,
        CancellationToken cancellationToken = default)
    {
        var templates = await _repository.ListAsync(cancellationToken);
        return Result.Success<IReadOnlyList<EmailTemplateDto>>(
            templates.Select(t => t.ToDto()).ToList().AsReadOnly());
    }
}
