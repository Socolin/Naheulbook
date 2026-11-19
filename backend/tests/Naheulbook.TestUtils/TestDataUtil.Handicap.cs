using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddHandicap(Action<HandicapEntity> customizer = null)
    {
        return AddHandicap(out _, customizer);
    }

    public TestDataUtil AddHandicap(out HandicapEntity handicap, Action<HandicapEntity> customizer = null)
    {
        handicap = new HandicapEntity
        {
            Id = Guid.NewGuid(),
            DiceRoll = Random.Shared.Next(1, 1_000_000),
            Name = RngUtil.GetRandomString("some-handicap-name"),
            Data = """{"condition": "some-condition", "effect": "some-effect"}""",
        };

        return SaveEntity(handicap, customizer);
    }

    public TestDataUtil AddCharacterHandicap(Action<CharacterHandicapEntity> customizer = null)
    {
        return AddCharacterHandicap(out _, customizer);
    }

    public TestDataUtil AddCharacterHandicap(out CharacterHandicapEntity characterHandicap, Action<CharacterHandicapEntity> customizer = null)
    {
        var character = GetLast<CharacterEntity>();
        var handicap = GetLast<HandicapEntity>();
        characterHandicap = new CharacterHandicapEntity
        {
            CharacterId = character.Id,
            HandicapId = handicap.Id,
        };

        return SaveEntity(characterHandicap, customizer);
    }
}
