namespace Mavrynt.Modules.Notifications.Application.Models;

public interface IEmailModel
{
    IReadOnlyDictionary<string, string> ToPlaceholders();
}
