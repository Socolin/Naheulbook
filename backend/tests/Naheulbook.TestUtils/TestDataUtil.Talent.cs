using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddTalentGroup(Action<TalentGroupEntity> customizer = null)
    {
        return AddTalentGroup(out _, customizer);
    }

    public TestDataUtil AddTalentGroup(out TalentGroupEntity talentGroup, Action<TalentGroupEntity> customizer = null)
    {
        talentGroup = new TalentGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = RngUtil.GetRandomString("some-talent-group-name"),
            Description = RngUtil.GetRandomString("some-talent-group-description"),
            DisplayOrder = Random.Shared.Next(1, 100),
            Talents = [],
        };

        return SaveEntity(talentGroup, customizer);
    }

    public TestDataUtil AddTalent(Action<TalentEntity> customizer = null)
    {
        return AddTalent(out _, customizer);
    }

    public TestDataUtil AddTalent(out TalentEntity talent, Action<TalentEntity> customizer = null)
    {
        var talentGroup = GetLast<TalentGroupEntity>();
        talent = new TalentEntity
        {
            Id = Guid.NewGuid(),
            TalentGroupId = talentGroup.Id,
            TalentGroup = talentGroup,
            DisplayOrder = Random.Shared.Next(1, 100),
            Name = RngUtil.GetRandomString("some-talent-name"),
            Data = """{"restriction": "some-restriction", "effect": "some-effect", "requiredLevel": 5}""",
        };

        return SaveEntity(talent, customizer);
    }

    public TestDataUtil AddCharacterTalent(Action<CharacterTalentEntity> customizer = null)
    {
        return AddCharacterTalent(out _, customizer);
    }

    public TestDataUtil AddCharacterTalent(out CharacterTalentEntity characterTalent, Action<CharacterTalentEntity> customizer = null)
    {
        var character = GetLast<CharacterEntity>();
        var talent = GetLast<TalentEntity>();
        characterTalent = new CharacterTalentEntity
        {
            CharacterId = character.Id,
            TalentId = talent.Id,
        };

        return SaveEntity(characterTalent, customizer);
    }
}
