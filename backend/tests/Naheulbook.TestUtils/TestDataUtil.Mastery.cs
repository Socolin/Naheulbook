using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddMasteryGroup(Action<MasteryGroupEntity> customizer = null)
    {
        return AddMasteryGroup(out _, customizer);
    }

    public TestDataUtil AddMasteryGroup(out MasteryGroupEntity masteryGroup, Action<MasteryGroupEntity> customizer = null)
    {
        masteryGroup = new MasteryGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = RngUtil.GetRandomString("some-mastery-group-name"),
            Description = RngUtil.GetRandomString("some-mastery-group-description"),
            DisplayOrder = Random.Shared.Next(1, 100),
            Masteries = [],
        };

        return SaveEntity(masteryGroup, customizer);
    }

    public TestDataUtil AddMastery(Action<MasteryEntity> customizer = null)
    {
        return AddMastery(out _, customizer);
    }

    public TestDataUtil AddMastery(out MasteryEntity mastery, Action<MasteryEntity> customizer = null)
    {
        var masteryGroup = GetLast<MasteryGroupEntity>();
        mastery = new MasteryEntity
        {
            Id = Guid.NewGuid(),
            MasteryGroupId = masteryGroup.Id,
            MasteryGroup = masteryGroup,
            DisplayOrder = Random.Shared.Next(1, 100),
            Name = RngUtil.GetRandomString("some-mastery-name"),
            Data = """{"restriction": "some-restriction", "effect": "some-effect", "requiredLevel": 10}""",
        };

        return SaveEntity(mastery, customizer);
    }

}
