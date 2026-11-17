using AwesomeAssertions;
using Naheulbook.Core.Features.Group;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Group;

[TestFixture]
[TestOf(typeof(DurationChangeMapper))]
public class DurationChangeMapperTests
{
    private DurationChangeMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new DurationChangeMapper();
    }

    [Test]
    public void ToFighterDurationChanges_WithMonsterId_ReturnsMonsterUpdateDuration()
    {
        var change = new DurationChangeRequest {Type = ModifierDurationChange.TypeValue, Modifier = new NewModifierDurationValue {Id = 1}};
        var request = new PostGroupUpdateDurationsRequest {MonsterId = 3, Changes = [change]};

        var actual = _mapper.ToFighterDurationChanges(request);

        actual.Should().BeOfType<MonsterUpdateDuration>()
            .Which.Should().BeEquivalentTo(new MonsterUpdateDuration
            {
                MonsterId = 3,
                Changes = [_mapper.ToDurationChange(change)],
            }, o => o.PreferringRuntimeMemberTypes());
    }

    [Test]
    public void ToFighterDurationChanges_WithCharacterId_ReturnsCharacterUpdateDuration()
    {
        var change = new DurationChangeRequest {Type = ModifierDurationChange.TypeValue, Modifier = new NewModifierDurationValue {Id = 1}};
        var request = new PostGroupUpdateDurationsRequest {CharacterId = 4, Changes = [change]};

        var actual = _mapper.ToFighterDurationChanges(request);

        actual.Should().BeOfType<CharacterUpdateDuration>()
            .Which.Should().BeEquivalentTo(new CharacterUpdateDuration
            {
                CharacterId = 4,
                Changes = [_mapper.ToDurationChange(change)],
            }, o => o.PreferringRuntimeMemberTypes());
    }

    [Test]
    public void ToFighterDurationChanges_WithoutMonsterIdNorCharacterId_Throws()
    {
        var request = new PostGroupUpdateDurationsRequest {Changes = []};

        var act = () => _mapper.ToFighterDurationChanges(request);

        act.Should().Throw<NotSupportedException>();
    }

    [Test]
    public void ToDurationChange_ItemModifier_ReturnsItemModifierDurationChange()
    {
        var modifier = new NewModifierDurationValue {Id = 1, Active = true, CurrentCombatCount = 2};
        var request = new DurationChangeRequest {Type = ItemModifierDurationChange.TypeValue, ItemId = 5, Modifier = modifier};

        var actual = _mapper.ToDurationChange(request);

        actual.Should().BeOfType<ItemModifierDurationChange>()
            .Which.Should().BeEquivalentTo(new ItemModifierDurationChange {ItemId = 5, Modifier = modifier});
    }

    [Test]
    public void ToDurationChange_ItemLifetime_ReturnsItemLifetimeDurationChange()
    {
        var lifeTime = new LifeTime();
        var request = new DurationChangeRequest {Type = ItemLifetimeDurationChange.TypeValue, ItemId = 5, LifeTime = lifeTime};

        var actual = _mapper.ToDurationChange(request);

        actual.Should().BeOfType<ItemLifetimeDurationChange>()
            .Which.Should().BeEquivalentTo(new ItemLifetimeDurationChange {ItemId = 5, LifeTime = lifeTime});
    }

    [Test]
    public void ToDurationChange_Modifier_ReturnsModifierDurationChange()
    {
        var modifier = new NewModifierDurationValue {Id = 1};
        var request = new DurationChangeRequest {Type = ModifierDurationChange.TypeValue, Modifier = modifier};

        var actual = _mapper.ToDurationChange(request);

        actual.Should().BeOfType<ModifierDurationChange>()
            .Which.Should().BeEquivalentTo(new ModifierDurationChange {Modifier = modifier});
    }

    [TestCase(ItemModifierDurationChange.TypeValue)]
    [TestCase(ItemLifetimeDurationChange.TypeValue)]
    public void ToDurationChange_ItemChangeWithoutItemId_Throws(string type)
    {
        var request = new DurationChangeRequest {Type = type, ItemId = null};

        var act = () => _mapper.ToDurationChange(request);

        act.Should().Throw<NotSupportedException>();
    }

    [Test]
    public void ToDurationChange_UnknownType_Throws()
    {
        var request = new DurationChangeRequest {Type = "some-unknown-type"};

        var act = () => _mapper.ToDurationChange(request);

        act.Should().Throw<NotSupportedException>();
    }
}
