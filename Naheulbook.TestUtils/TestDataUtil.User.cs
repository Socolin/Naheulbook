using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddUser(Action<User> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateUser(), customizer);
        }

        public TestDataUtil AddUserAccessToken(Action<UserAccessToken> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateUserAccessToken(GetLast<User>()), customizer);
        }
    }
}