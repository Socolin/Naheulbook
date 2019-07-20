using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class InvalidTargetLevelUpRequestedException : Exception
    {
        public int CurrentLevel { get; }
        public int TargetLevel { get; }

        public InvalidTargetLevelUpRequestedException(int currentLevel, int targetLevel)
        {
            CurrentLevel = currentLevel;
            TargetLevel = targetLevel;
        }
    }
}