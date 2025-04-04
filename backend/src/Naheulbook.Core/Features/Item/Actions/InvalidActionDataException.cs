using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Item.Actions;

public class InvalidActionDataException : Exception
{
    public string ActionType { get; }

    public InvalidActionDataException(string actionType)
    {
        ActionType = actionType;
        throw new NotImplementedException();
    }
}