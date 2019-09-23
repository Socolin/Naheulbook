using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class InvalidActionTypeException : Exception
    {
        public string ActionType { get; }
        public string ExpectedActionType { get; }

        public InvalidActionTypeException(string actionType, string expectedActionType)
        {
            ActionType = actionType;
            ExpectedActionType = expectedActionType;
        }
    }
}