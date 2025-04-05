

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Item.Actions;

public class InvalidActionTypeException(string actionType, string expectedActionType) : Exception
{
    public string ActionType { get; } = actionType;
    public string ExpectedActionType { get; } = expectedActionType;
}