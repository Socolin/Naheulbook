using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddUser(Action<UserEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateUser(), customizer);
        }

        public TestDataUtil AddUserAccessToken(Action<UserAccessTokenEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateUserAccessToken(GetLast<UserEntity>()), customizer);
        }
    }
}