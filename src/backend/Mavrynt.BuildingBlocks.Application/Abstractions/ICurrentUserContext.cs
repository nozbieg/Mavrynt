namespace Mavrynt.BuildingBlocks.Application.Abstractions;

public interface ICurrentUserContext
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}
