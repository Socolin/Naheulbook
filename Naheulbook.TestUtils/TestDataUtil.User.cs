using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddUser(Action<UserEntity> customizer = null)
        {
            return AddUser(out _, customizer);
        }

        public TestDataUtil AddUser(out UserEntity user, Action<UserEntity> customizer = null)
        {
            user = _defaultEntityCreator.CreateUser();
            return SaveEntity(user, customizer);
        }

        public TestDataUtil AddUserAccessToken(Action<UserAccessTokenEntity> customizer = null)
        {
            return AddUserAccessToken(out _, customizer);
        }

        public TestDataUtil AddUserAccessToken(out UserAccessTokenEntity userAccessToken, Action<UserAccessTokenEntity> customizer = null)
        {
            userAccessToken = _defaultEntityCreator.CreateUserAccessToken(GetLast<UserEntity>());
            return SaveEntity(userAccessToken, customizer);
        }
    }
}