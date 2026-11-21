using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddMankdebolCalamity(Action<MankdebolCalamityEntity> customizer = null)
    {
        return AddMankdebolCalamity(out _, customizer);
    }

    public TestDataUtil AddMankdebolCalamity(out MankdebolCalamityEntity mankdebolCalamity, Action<MankdebolCalamityEntity> customizer = null)
    {
        mankdebolCalamity = new MankdebolCalamityEntity
        {
            Id = Guid.NewGuid(),
            DiceRoll = Random.Shared.Next(1, 101),
            Name = RngUtil.GetRandomString("some-calamity-name"),
            Effect = RngUtil.GetRandomString("some-calamity-effect"),
        };

        return SaveEntity(mankdebolCalamity, customizer);
    }
}
