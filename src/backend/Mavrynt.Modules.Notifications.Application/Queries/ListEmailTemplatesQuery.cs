using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed record ListEmailTemplatesQuery : IQuery<IReadOnlyList<EmailTemplateDto>>;
