using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Character CreateCharacter(int ownerId, Origin origin, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Character
            {
                Name = $"some-character-name-{suffix}",
                Sex = "homme",
                IsActive = true,
                IsNpc = false,

                Ad = 3,
                Cha = 4,
                Cou = 5,
                Fo = 6,
                Int = 7,

                Level = 1,
                Experience = 0,

                OriginId = origin.Id,
                OwnerId = ownerId
            };
        }
    }
}