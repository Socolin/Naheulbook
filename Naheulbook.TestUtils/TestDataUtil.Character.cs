using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddCharacter(int ownerId, Action<Character> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateCharacter(ownerId, GetLast<Origin>()), customizer);
        }
    }
}