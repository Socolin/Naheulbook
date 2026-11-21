using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddDebilibeukBlessing(Action<DebilibeukBlessingEntity> customizer = null)
    {
        return AddDebilibeukBlessing(out _, customizer);
    }

    public TestDataUtil AddDebilibeukBlessing(out DebilibeukBlessingEntity debilibeukBlessing, Action<DebilibeukBlessingEntity> customizer = null)
    {
        debilibeukBlessing = new DebilibeukBlessingEntity
        {
            Id = Guid.NewGuid(),
            DiceRoll = Random.Shared.Next(1, 101),
            Name = RngUtil.GetRandomString("some-blessing-name"),
            Effect = RngUtil.GetRandomString("some-blessing-effect"),
        };

        return SaveEntity(debilibeukBlessing, customizer);
    }
}
