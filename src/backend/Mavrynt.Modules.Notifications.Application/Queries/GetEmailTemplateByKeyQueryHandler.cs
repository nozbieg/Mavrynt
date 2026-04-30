using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Application.Mapping;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed class GetEmailTemplateByKeyQueryHandler : IQueryHandler<GetEmailTemplateByKeyQuery, EmailTemplateDto>
{
    private readonly IEmailTemplateRepository _repository;

    public GetEmailTemplateByKeyQueryHandler(IEmailTemplateRepository repository)
        => _repository = repository;

    public async Task<Result<EmailTemplateDto>> HandleAsync(
        GetEmailTemplateByKeyQuery query,
        CancellationToken cancellationToken = default)
    {
        var keyResult = EmailTemplateKey.Create(query.TemplateKey);
        if (keyResult.IsFailure) return keyResult.Error;

        var template = await _repository.GetByKeyAsync(keyResult.Value, cancellationToken);
        if (template is null) return NotificationsErrors.EmailTemplateNotFound;

        return template.ToDto();
    }
}
