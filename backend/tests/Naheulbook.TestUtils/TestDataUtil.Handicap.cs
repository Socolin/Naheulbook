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
}
