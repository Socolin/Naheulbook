using Naheulbook.Core.Features.Group;
using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class GroupSteps(TestDataUtil testDataUtil)
{
    [Given("^a group$")]
    public void GivenAGroup()
    {
        testDataUtil.AddGroup();
    }

    [Given("^a group history entry$")]
    public void GivenAGroupHistoryEntry()
    {
        testDataUtil.AddGroupHistoryEntry();
    }

    [Given("^an event$")]
    public void GivenAnEvent()
    {
        testDataUtil.AddEvent();
    }

    [Given("^a loot$")]
    public void GivenALoot()
    {
        testDataUtil.AddLoot();
    }

    [Given("^that the loot is visible for players$")]
    public void GivenThatTheLootIsVisibleForPlayers()
    {
        testDataUtil.GetLast<LootEntity>().IsVisibleForPlayer = true;
        testDataUtil.SaveChanges();
    }

    [Given("^an invite from the group to the (.+) character$")]
    public void GivenAnInviteFromTheGroupToTheXCharacter(string indexString)
    {
        var character = testDataUtil.Get<CharacterEntity>(StepArgumentUtil.ParseIndex(indexString));
        var group = testDataUtil.GetLast<GroupEntity>();

        testDataUtil.AddGroupInvite(character, group, true);
    }

    [Given("^a request from (.+) character to join the group$")]
    public void GivenARequestFromXCharacterToJoinTheGroup(string indexString)
    {
        var character = testDataUtil.Get<CharacterEntity>(StepArgumentUtil.ParseIndex(indexString));
        var group = testDataUtil.GetLast<GroupEntity>();

        testDataUtil.AddGroupInvite(character, group, false);
    }

    [Given("^that the loot is the current group combat loot$")]
    public void GivenThatTheLootIsTheCurrentGroupCombatLoot()
    {
        testDataUtil.GetLast<GroupEntity>().CombatLootId = testDataUtil.GetLast<LootEntity>().Id;
        testDataUtil.SaveChanges();
    }

    [Given(@"^that the group have a date set to the (.+) day of the year (\d+) at (\d+):(\d+)$")]
    public void GivenThatTheGroupHaveADateSet(string day, int year, int hour, int minute)
    {
        var group = testDataUtil.GetLast<GroupEntity>();
        var groupData = JsonConvert.DeserializeObject<GroupData>(group.Data ?? "null") ?? new GroupData();
        groupData.Date = new NhbkDate
        {
            Day = StepArgumentUtil.ParseNth(day),
            Hour = hour,
            Minute = minute,
            Year = year,
        };

        group.Data = JsonConvert.SerializeObject(groupData,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        testDataUtil.SaveChanges();
    }

    [Given("^a npc$")]
    public void GivenANpc()
    {
        testDataUtil.AddNpc();
    }

    [Given(@"^a prepared fight$")]
    public void GivenAPreparedFight()
    {
        testDataUtil.AddFight();
    }

    [Given(@"^a monster for the fight$")]
    public void GivenAMonsterForTheFight()
    {
        testDataUtil.AddMonster(m => m.FightId = testDataUtil.GetLast<FightEntity>().Id);
    }
}