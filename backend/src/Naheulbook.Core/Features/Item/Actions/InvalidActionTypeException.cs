using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Item.Actions;

public class InvalidActionTypeException(string actionType, string expectedActionType) : Exception
{
    public string ActionType { get; } = actionType;
    public string ExpectedActionType { get; } = expectedActionType;
}