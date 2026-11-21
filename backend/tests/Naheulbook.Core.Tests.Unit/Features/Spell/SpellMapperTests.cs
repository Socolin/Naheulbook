using AwesomeAssertions;
using Naheulbook.Core.Features.Spell;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Spell;

[TestFixture]
[TestOf(typeof(SpellMapper))]
public class SpellMapperTests
{
    private static readonly Guid ItemTemplateId = Guid.Parse("0a3d2c1b-8e7f-4a6b-9c5d-1e2f3a4b5c6d");
    private SpellMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new SpellMapper();
    }

    [Test]
    public void ToResponse_Spell_MapsAllFields()
    {
        var spell = CreateSpell("some-name", 1);

        var actual = _mapper.ToResponse(spell);

        actual.Should().BeEquivalentTo(new SpellResponse
        {
            Id = spell.Id,
            Name = "some-name",
            Level = 4,
            LevelLabel = "Niveau 4",
            Kind = "invocation",
            Category = "some-category",
            Description = "some-description",
            Data = new SpellData
            {
                Tags = ["combat"],
                Usages = ["some-usage"],
                CastingTime = new SpellCastingTime {Text = "2 assauts", Kind = "fixed", Value = 2, Unit = "assault", Type = "incantation", Notes = ["some-casting-note"]},
                Duration = new SpellTime {Text = "1D20 minutes", Kind = "fixed", Dice = "1D20", Unit = "minute"},
                Cost = new SpellCost {Text = "2 PA / assaut", Kind = "per_unit", AstralPoints = 2, MinAstralPoints = 1, MaxAstralPoints = 10, Per = "assaut", Notes = ["some-cost-note"]},
                Test = new SpellTest
                {
                    Text = "MagiePsy-2", Kind = "confrontation", Stat = "MagiePsy", Modifier = -2, RequiredUntilLevel = 8,
                    Confrontation = new SpellConfrontation {Caster = "MagiePsy+2D6", Target = "INT+3D6"},
                    MagicResistance = "some-magic-resistance",
                },
                Range = new SpellRange {Text = "Jusqu’à 8 m", Kind = "distance", Meters = 8},
                Area = new SpellText {Text = "cercle 5 m", Notes = ["some-area-note"]},
                Damage = new SpellDamage
                {
                    Text = "2D+2 par cible", Dice = "2D+2", PerTarget = true, Fixed = true, IgnoresArmor = true,
                    ByLevel = [new SpellDamageByLevel {MinLevel = 1, MaxLevel = 2, Value = "1D+2"}, new SpellDamageByLevel {MinLevel = 3, Value = "2D"}],
                },
                Benefits = ["some-benefit"],
                Effects = ["some-effect"],
                Restrictions = ["some-restriction"],
                Ingredients = [new SpellIngredient {Name = "poudre d’argent", Quantity = 1, Unit = "dose", Text = "1 dose de poudre d’argent", ItemTemplateId = ItemTemplateId}],
                RitualDetails = "some-ritual-details",
                LevelDetails = "some-level-details",
                PowerWord = ["some-power-word"],
                Characteristics = ["some-characteristic"],
                Invocation = new SpellInvocation
                {
                    CallTime = new SpellTime {Text = "2 assauts", Kind = "fixed", Value = 2, Unit = "assault"},
                    CallTest = new SpellTest {Text = "MagiePsy", Kind = "simple", Stat = "MagiePsy", Modifier = 0},
                    ControlTest = new SpellInvocationTest {Required = true, Caster = "MagiePsy+2D6", Target = "2D6+6", Notes = ["some-control-note"]},
                    RevokeTest = new SpellInvocationTest {Text = "non", Required = false},
                    MaxCount = 1,
                    MaxCountDetails = new SpellText {Text = "1", Notes = ["some-max-count-note"]},
                    CreatureStats = new Dictionary<string, string> {["ev"] = "10", ["at"] = "9"},
                    MonsterTemplateId = 42,
                },
                CriticalSuccess = "some-critical-success",
                CriticalFailure = "some-critical-failure",
                CriticalNotes = "some-critical-notes",
                Notes = "some-notes",
                Source = new SpellSource {Pdf = "some.pdf", Pages = [5, 6]},
            },
        });
    }

    [Test]
    public void ToResponse_SpellBook_MapsFieldsAndSpellsOrderedByDisplayOrder()
    {
        var spell2 = CreateSpell("some-name-2", 2);
        var spell1 = CreateSpell("some-name-1", 1);
        var spellBook = new SpellBookEntity
        {
            Id = Guid.NewGuid(),
            TechName = "some-tech-name",
            Name = "some-book-name",
            Kind = "priest",
            JobId = Guid.NewGuid(),
            SpecialityId = Guid.NewGuid(),
            GodId = 2,
            Data = """{"description": "some-book-description", "categories": ["a", "b"], "sourcePdf": "some.pdf"}""",
            Spells = [spell2, spell1],
        };

        var actual = _mapper.ToResponse(spellBook);

        actual.Should().BeEquivalentTo(new SpellBookResponse
        {
            Id = spellBook.Id,
            TechName = "some-tech-name",
            Name = "some-book-name",
            Kind = "priest",
            JobId = spellBook.JobId,
            SpecialityId = spellBook.SpecialityId,
            GodId = 2,
            Data = new SpellBookData {Description = "some-book-description", Categories = ["a", "b"], SourcePdf = "some.pdf"},
        }, o => o.Excluding(x => x.Spells));
        actual.Spells.Select(x => x.Name).Should().Equal("some-name-1", "some-name-2");
    }

    private static SpellEntity CreateSpell(string name, int displayOrder)
    {
        return new SpellEntity
        {
            Id = Guid.NewGuid(),
            DisplayOrder = displayOrder,
            Name = name,
            Level = 4,
            LevelLabel = "Niveau 4",
            Kind = "invocation",
            Category = "some-category",
            Description = "some-description",
            Data = """
                   {
                     "tags": ["combat"],
                     "usages": ["some-usage"],
                     "castingTime": {"text": "2 assauts", "kind": "fixed", "value": 2, "unit": "assault", "type": "incantation", "notes": ["some-casting-note"]},
                     "duration": {"text": "1D20 minutes", "kind": "fixed", "dice": "1D20", "unit": "minute"},
                     "cost": {"text": "2 PA / assaut", "kind": "per_unit", "astralPoints": 2, "minAstralPoints": 1, "maxAstralPoints": 10, "per": "assaut", "notes": ["some-cost-note"]},
                     "test": {
                       "text": "MagiePsy-2", "kind": "confrontation", "stat": "MagiePsy", "modifier": -2, "requiredUntilLevel": 8,
                       "confrontation": {"caster": "MagiePsy+2D6", "target": "INT+3D6"},
                       "magicResistance": "some-magic-resistance"
                     },
                     "range": {"text": "Jusqu’à 8 m", "kind": "distance", "meters": 8},
                     "area": {"text": "cercle 5 m", "notes": ["some-area-note"]},
                     "damage": {
                       "text": "2D+2 par cible", "dice": "2D+2", "perTarget": true, "fixed": true, "ignoresArmor": true,
                       "byLevel": [{"minLevel": 1, "maxLevel": 2, "value": "1D+2"}, {"minLevel": 3, "value": "2D"}]
                     },
                     "benefits": ["some-benefit"],
                     "effects": ["some-effect"],
                     "restrictions": ["some-restriction"],
                     "ingredients": [{"name": "poudre d’argent", "quantity": 1, "unit": "dose", "text": "1 dose de poudre d’argent", "itemTemplateId": "0a3d2c1b-8e7f-4a6b-9c5d-1e2f3a4b5c6d"}],
                     "ritualDetails": "some-ritual-details",
                     "levelDetails": "some-level-details",
                     "powerWord": ["some-power-word"],
                     "characteristics": ["some-characteristic"],
                     "invocation": {
                       "callTime": {"text": "2 assauts", "kind": "fixed", "value": 2, "unit": "assault"},
                       "callTest": {"text": "MagiePsy", "kind": "simple", "stat": "MagiePsy", "modifier": 0},
                       "controlTest": {"required": true, "caster": "MagiePsy+2D6", "target": "2D6+6", "notes": ["some-control-note"]},
                       "revokeTest": {"text": "non", "required": false},
                       "maxCount": 1,
                       "maxCountDetails": {"text": "1", "notes": ["some-max-count-note"]},
                       "creatureStats": {"ev": "10", "at": "9"},
                       "monsterTemplateId": 42
                     },
                     "criticalSuccess": "some-critical-success",
                     "criticalFailure": "some-critical-failure",
                     "criticalNotes": "some-critical-notes",
                     "notes": "some-notes",
                     "source": {"pdf": "some.pdf", "pages": [5, 6]}
                   }
                   """,
        };
    }
}
