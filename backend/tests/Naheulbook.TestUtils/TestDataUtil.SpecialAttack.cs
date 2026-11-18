using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddSpecialAttackGroup(Action<SpecialAttackGroupEntity> customizer = null)
    {
        return AddSpecialAttackGroup(out _, customizer);
    }

    public TestDataUtil AddSpecialAttackGroup(out SpecialAttackGroupEntity specialAttackGroup, Action<SpecialAttackGroupEntity> customizer = null)
    {
        specialAttackGroup = new SpecialAttackGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = RngUtil.GetRandomString("some-special-attack-group-name"),
            Description = RngUtil.GetRandomString("some-special-attack-group-description"),
            DisplayOrder = Random.Shared.Next(1, 100),
            SpecialAttacks = [],
        };

        return SaveEntity(specialAttackGroup, customizer);
    }

    public TestDataUtil AddSpecialAttack(Action<SpecialAttackEntity> customizer = null)
    {
        return AddSpecialAttack(out _, customizer);
    }

    public TestDataUtil AddSpecialAttack(out SpecialAttackEntity specialAttack, Action<SpecialAttackEntity> customizer = null)
    {
        var specialAttackGroup = GetLast<SpecialAttackGroupEntity>();
        specialAttack = new SpecialAttackEntity
        {
            Id = Guid.NewGuid(),
            SpecialAttackGroupId = specialAttackGroup.Id,
            SpecialAttackGroup = specialAttackGroup,
            DisplayOrder = Random.Shared.Next(1, 100),
            Name = RngUtil.GetRandomString("some-special-attack-name"),
            Data = """{"requiredLevel": 3, "usableBy": "some-usable-by", "effects": "some-effects", "xp": 2}""",
        };

        return SaveEntity(specialAttack, customizer);
    }
}
