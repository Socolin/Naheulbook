using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class GroupSteps
    {
        private readonly TestDataUtil _testDataUtil;
        private readonly ScenarioContext _scenarioContext;

        public GroupSteps(TestDataUtil testDataUtil, ScenarioContext scenarioContext)
        {
            _testDataUtil = testDataUtil;
            _scenarioContext = scenarioContext;
        }

        [Given("a group")]
        public void GivenAGroup()
        {
            _testDataUtil.AddGroup(_scenarioContext.GetUserId());
        }

        [Given("a group history entry")]
        public void GivenAGroupHistoryEntry()
        {
            _testDataUtil.AddGroupHistoryEntry();
        }

        [Given("an event")]
        public void GivenAnEvent()
        {
            _testDataUtil.AddEvent();
        }

        [Given("a loot")]
        public void GivenALoot()
        {
            _testDataUtil.AddLoot();
        }

        [Given("that the loot is visible for players")]
        public void GivenThatTheLootIsVisibleForPlayers()
        {
            _testDataUtil.GetLast<Loot>().IsVisibleForPlayer = true;
            _testDataUtil.SaveChanges();
        }

        [Given("an invite from the group to the (.+) character")]
        public void GivenAnInviteFromTheGroupToTheXCharacter(string indexString)
        {
            var character = _testDataUtil.Get<Character>(StepArgumentUtil.ParseIndex(indexString));
            var group = _testDataUtil.GetLast<Group>();

            _testDataUtil.AddGroupInvite(character, group, true);
        }

        [Given("a request from (.+) character to join the group")]
        public void GivenARequestFromXCharacterToJoinTheGroup(string indexString)
        {
            var character = _testDataUtil.Get<Character>(StepArgumentUtil.ParseIndex(indexString));
            var group = _testDataUtil.GetLast<Group>();

            _testDataUtil.AddGroupInvite(character, group, false);
        }

        [Given("that the loot is the current group combat loot")]
        public void GivenThatTheLootIsTheCurrentGroupCombatLoot()
        {
            _testDataUtil.GetLast<Group>().CombatLootId = _testDataUtil.GetLast<Loot>().Id;
            _testDataUtil.SaveChanges();
        }

        [Given(@"that the group have a date set to the (.+) day of the year (\d+) at (\d+):(\d+)")]
        public void GivenThatTheGroupHaveADateSet(string day, int year, int hour, int minute)
        {
            var group = _testDataUtil.GetLast<Group>();
            var groupData = JsonConvert.DeserializeObject<GroupData>(group.Data) ?? new GroupData();
            groupData.Date = new NhbkDate
            {
                Day = StepArgumentUtil.ParseNth(day),
                Hour = hour,
                Minute = minute,
                Year = year
            };

            group.Data = JsonConvert.SerializeObject(groupData, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            _testDataUtil.SaveChanges();
        }

        [Given("a npc")]
        public void GivenANpc()
        {
            _testDataUtil.AddNpc();
        }
    }
}