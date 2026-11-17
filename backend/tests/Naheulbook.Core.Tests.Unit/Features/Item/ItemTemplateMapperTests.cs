using AwesomeAssertions;
using Naheulbook.Core.Features.Item;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Item;

[TestFixture]
[TestOf(typeof(ItemTemplateMapper))]
public class ItemTemplateMapperTests
{
    private ItemTemplateMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new ItemTemplateMapper();
    }

    [Test]
    public void ToResponse_ItemTemplate_MapsAllFields()
    {
        var skillId = Guid.NewGuid();
        var unSkillId = Guid.NewGuid();
        var modifier = new ItemTemplateModifierEntity {StatName = "AD", Type = "ADD", Value = 1};
        var skillModifier = new ItemTemplateSkillModifierEntity {SkillId = Guid.NewGuid(), Value = 2};
        var requirement = new ItemTemplateRequirementEntity {StatName = "FO", MinValue = 1, MaxValue = 2};
        var slot = new SlotEntity {Id = 6, Name = "some-slot", TechName = "SOME_SLOT"};
        var itemTemplate = new ItemTemplateEntity
        {
            Id = Guid.NewGuid(),
            SubCategoryId = 3,
            Name = "some-name",
            CleanName = "some-clean-name",
            TechName = "some-tech-name",
            Source = "official",
            SourceUserId = 4,
            SourceUserNameCache = "some-source-user-name",
            Data = """{"price":12}""",
            Modifiers = [modifier],
            Skills = [new ItemTemplateSkillEntity {SkillId = skillId}],
            UnSkills = [new ItemTemplateUnSkillEntity {SkillId = unSkillId}],
            SkillModifiers = [skillModifier],
            Requirements = [requirement],
            Slots = [new ItemTemplateSlotEntity {SlotId = 6, Slot = slot}],
        };

        var actual = _mapper.ToResponse(itemTemplate);

        actual.Should().BeEquivalentTo(new ItemTemplateResponse
        {
            Id = itemTemplate.Id,
            SubCategoryId = 3,
            Name = "some-name",
            TechName = "some-tech-name",
            Source = "official",
            SourceUserId = 4,
            SourceUser = "some-source-user-name",
            Data = new JObject {["price"] = 12},
            Modifiers = [_mapper.ToResponse(modifier)],
            SkillIds = [skillId],
            UnSkillIds = [unSkillId],
            SkillModifiers = [_mapper.ToResponse(skillModifier)],
            Requirements = [_mapper.ToResponse(requirement)],
            Slots = [_mapper.ToResponse(slot)],
        });
    }

    [Test]
    public void ToResponse_ItemTemplateWithoutData_ReturnsEmptyData()
    {
        var itemTemplate = new ItemTemplateEntity
        {
            Name = "some-name",
            Source = "official",
            Data = null,
            Modifiers = [],
            Skills = [],
            UnSkills = [],
            SkillModifiers = [],
            Requirements = [],
            Slots = [],
        };

        var actual = _mapper.ToResponse(itemTemplate);

        actual.Data.Should().BeEquivalentTo(new JObject());
    }

    [Test]
    public void ToResponse_ItemTemplateModifier_MapsAllFields()
    {
        var modifier = new ItemTemplateModifierEntity
        {
            Id = 3,
            StatName = "AD",
            Type = "ADD",
            Value = 4,
            Special = "a,b",
            RequiredJobId = Guid.NewGuid(),
            RequiredOriginId = Guid.NewGuid(),
        };

        var actual = _mapper.ToResponse(modifier);

        actual.Should().BeEquivalentTo(new ItemTemplateModifierResponse
        {
            Stat = "AD",
            Type = "ADD",
            Value = 4,
            Special = ["a", "b"],
            JobId = modifier.RequiredJobId,
            OriginId = modifier.RequiredOriginId,
        });
    }

    [Test]
    public void ToResponse_ItemTemplateModifierWithoutSpecial_ReturnsEmptySpecial()
    {
        var modifier = new ItemTemplateModifierEntity {StatName = "AD", Type = "ADD", Special = null};

        var actual = _mapper.ToResponse(modifier);

        actual.Special.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToResponse_ItemTemplateRequirement_MapsStatAndMinMax()
    {
        var requirement = new ItemTemplateRequirementEntity {StatName = "FO", MinValue = 8, MaxValue = 12};

        var actual = _mapper.ToResponse(requirement);

        actual.Should().BeEquivalentTo(new ItemTemplateRequirementResponse
        {
            Stat = "FO",
            Min = 8,
            Max = 12,
        });
    }

    [Test]
    public void ToResponse_ItemTemplateRequirementWithoutMinMax_DefaultsToZero()
    {
        var requirement = new ItemTemplateRequirementEntity {StatName = "FO", MinValue = null, MaxValue = null};

        var actual = _mapper.ToResponse(requirement);

        actual.Min.Should().Be(0);
        actual.Max.Should().Be(0);
    }

    [Test]
    public void ToResponse_ItemTemplateSkillModifier_MapsSkillIdAndValue()
    {
        var skillModifier = new ItemTemplateSkillModifierEntity {Id = 3, SkillId = Guid.NewGuid(), Value = -2};

        var actual = _mapper.ToResponse(skillModifier);

        actual.Should().BeEquivalentTo(new ItemTemplateSkillModifierResponse
        {
            SkillId = skillModifier.SkillId,
            Value = -2,
        });
    }

    [Test]
    public void ToResponse_Slot_MapsIdNameAndTechName()
    {
        var slot = new SlotEntity {Id = 3, Name = "some-name", TechName = "SOME_TECH_NAME", Count = 2, Stackable = true};

        var actual = _mapper.ToResponse(slot);

        actual.Should().BeEquivalentTo(new ItemSlotResponse
        {
            Id = 3,
            Name = "some-name",
            TechName = "SOME_TECH_NAME",
        });
    }

    [Test]
    public void ToResponse_ItemTemplateSection_MapsFieldsAndOrdersSubCategoriesById()
    {
        var subCategory1 = CreateSubCategory(1);
        var subCategory2 = CreateSubCategory(2);
        var section = new ItemTemplateSectionEntity
        {
            Id = 3,
            Name = "some-name",
            Note = "some-note",
            Icon = "some-icon",
            Special = "a,b",
            SubCategories = [subCategory2, subCategory1],
        };

        var actual = _mapper.ToResponse(section);

        actual.Should().BeEquivalentTo(new ItemTemplateSectionResponse
        {
            Id = 3,
            Name = "some-name",
            Note = "some-note",
            Icon = "some-icon",
            Specials = ["a", "b"],
            SubCategories = [_mapper.ToResponse(subCategory1), _mapper.ToResponse(subCategory2)],
        }, o => o.WithStrictOrdering());
    }

    [Test]
    public void ToResponse_ItemTemplateSectionWithEmptySpecial_ReturnsEmptySpecials()
    {
        var section = new ItemTemplateSectionEntity
        {
            Name = "some-name",
            Note = "some-note",
            Icon = "some-icon",
            Special = "",
            SubCategories = [],
        };

        var actual = _mapper.ToResponse(section);

        actual.Specials.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToResponse_ItemTemplateSubCategory_MapsAllFields()
    {
        var subCategory = CreateSubCategory(3);

        var actual = _mapper.ToResponse(subCategory);

        actual.Should().BeEquivalentTo(new ItemTemplateSubCategoryResponse
        {
            Id = 3,
            SectionId = 8,
            Name = "some-name-3",
            Description = "some-description",
            TechName = "some-tech-name",
            Note = "some-note",
        });
    }

    [Test]
    public void ToResponse_ItemType_MapsAllFields()
    {
        var itemType = new ItemTypeEntity {Id = 3, DisplayName = "some-display-name", TechName = "some-tech-name"};

        var actual = _mapper.ToResponse(itemType);

        actual.Should().BeEquivalentTo(new ItemTypeResponse
        {
            Id = 3,
            DisplayName = "some-display-name",
            TechName = "some-tech-name",
        });
    }

    [Test]
    public void ToEntity_ItemTemplateRequest_MapsAllFields()
    {
        var skillId = Guid.NewGuid();
        var unSkillId = Guid.NewGuid();
        var skillModifierSkillId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var originId = Guid.NewGuid();
        var request = new ItemTemplateRequest
        {
            Source = "community",
            SubCategoryId = 3,
            Name = "Épée longue",
            TechName = "some-tech-name",
            Data = new JObject {["price"] = 12},
            Modifiers =
            [
                new ItemTemplateModifierRequest {Stat = "AD", Type = "ADD", Value = 2, Special = ["a", "b"], JobId = jobId, OriginId = originId},
                new ItemTemplateModifierRequest {Stat = "FO", Type = "ADD", Value = 1, Special = null},
            ],
            SkillIds = [skillId],
            UnSkillIds = [unSkillId],
            SkillModifiers = [new ItemTemplateSkillModifierRequest {SkillId = skillModifierSkillId, Value = 3}],
            Requirements = [new ItemTemplateRequirementRequest {Stat = "CHA", Min = 4, Max = 8}],
            Slots = [new IdRequest {Id = 6}],
        };

        var actual = _mapper.ToEntity(request);

        actual.Should().BeEquivalentTo(new ItemTemplateEntity
        {
            Name = "Épée longue",
            CleanName = "Epee longue",
            TechName = "some-tech-name",
            Source = "community",
            SubCategoryId = 3,
            Data = """{"price":12}""",
            Modifiers =
            [
                new ItemTemplateModifierEntity {StatName = "AD", Type = "ADD", Value = 2, Special = "a,b", RequiredJobId = jobId, RequiredOriginId = originId},
                new ItemTemplateModifierEntity {StatName = "FO", Type = "ADD", Value = 1, Special = null},
            ],
            Skills = [new ItemTemplateSkillEntity {SkillId = skillId}],
            UnSkills = [new ItemTemplateUnSkillEntity {SkillId = unSkillId}],
            SkillModifiers = [new ItemTemplateSkillModifierEntity {SkillId = skillModifierSkillId, Value = 3}],
            Requirements = [new ItemTemplateRequirementEntity {StatName = "CHA", MinValue = 4, MaxValue = 8}],
            Slots = [new ItemTemplateSlotEntity {SlotId = 6}],
        });
    }

    private static ItemTemplateSubCategoryEntity CreateSubCategory(int id)
    {
        return new ItemTemplateSubCategoryEntity
        {
            Id = id,
            SectionId = 8,
            Name = $"some-name-{id}",
            Description = "some-description",
            TechName = "some-tech-name",
            Note = "some-note",
        };
    }
}
