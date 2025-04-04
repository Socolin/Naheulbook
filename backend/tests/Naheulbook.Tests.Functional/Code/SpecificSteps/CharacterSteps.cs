using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class CharacterSteps(
    TestDataUtil testDataUtil
)
{
    [Given(@"^(a|\d+) characters?$")]
    public void GivenACharacter(string amount)
    {
        if (!testDataUtil.Contains<OriginEntity>())
            testDataUtil.AddOrigin();
        if (testDataUtil.Contains<JobEntity>())
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            {
                testDataUtil.AddCharacter(c =>
                    {
                        c.GroupId = null;
                        c.Jobs = new List<CharacterJobEntity>
                        {
                            new() {JobId = testDataUtil.GetLast<JobEntity>().Id},
                        };
                    }
                );
            }
        }
        else
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                testDataUtil.AddCharacter(c => c.GroupId = null);
        }
    }

    [Given(@"^a character modifier$")]
    public void GivenACharacterModifier()
    {
        testDataUtil.AddCharacterModifier();
    }

    [Given(@"^an (inactive|active) (non-reusable|reusable) character modifier active for (\d) lap$")]
    public void GivenACharacterModifier(string active, string reusable, int lapCount)
    {
        testDataUtil.AddCharacterModifier(c =>
            {
                c.Reusable = reusable == "reusable";
                c.IsActive = active == "active";
                c.LapCount = lapCount;
                c.CurrentLapCount = lapCount;
            }
        );
    }

    [Given(@"^an inactive reusable character modifier that last 2 combat$")]
    public void GivenAnInactiveReusableCharacterModifierThatLast2Combat()
    {
        testDataUtil.AddCharacterModifier(c =>
            {
                c.Reusable = true;
                c.IsActive = false;
                c.CombatCount = 2;
            }
        );
    }

    [Given(@"^a character with all possible data$")]
    public void GivenACharacterWithAllPossibleData()
    {
        testDataUtil.AddCharacterWithAllData();
    }

    [Given(@"^an item based on that item template in the character inventory$")]
    public void GivenAnItemBasedOnThatItemTemplateInTheCharacterInventory()
    {
        testDataUtil.AddItem(testDataUtil.GetLast<CharacterEntity>());
        testDataUtil.GetLast<CharacterEntity>().Items = new List<ItemEntity>
        {
            testDataUtil.GetLast<ItemEntity>(),
        };
        testDataUtil.SaveChanges();
    }

    [Given(@"^an item based on that item template in the character inventory with (\d+) charges?$")]
    public void GivenAnItemBasedOnThatItemTemplateInTheCharacterInventoryWithXCharge(int chargeCount)
    {
        testDataUtil.AddItem(testDataUtil.GetLast<CharacterEntity>(), item => { item.Data = JsonConvert.SerializeObject(new {charge = chargeCount}); });

        testDataUtil.GetLast<CharacterEntity>().Items = new List<ItemEntity>
        {
            testDataUtil.GetLast<ItemEntity>(),
        };
        testDataUtil.SaveChanges();
    }

    [Given(@"^an item in the loot$")]
    public void GivenAnItemInTheLoot()
    {
        if (!testDataUtil.Contains<ItemTemplateEntity>())
            testDataUtil.AddItemTemplateSection().AddItemTemplateSubCategory().AddItemTemplate();
        testDataUtil.AddItemToLoot();
    }

    [Given(@"^an item in the character inventory$")]
    public void GivenAnItemInTheCharacterInventory()
    {
        if (!testDataUtil.Contains<ItemTemplateEntity>())
            testDataUtil.AddItemTemplateSection().AddItemTemplateSubCategory().AddItemTemplate();
        testDataUtil.AddItemToCharacter();
    }

    [Given("^that the (.+) character is a member of the group$")]
    public void GivenThatXTheCharacterIsAMemberOfTheGroup(string indexString)
    {
        var character = testDataUtil.Get<CharacterEntity>(StepArgumentUtil.ParseIndex(indexString));
        character.GroupId = testDataUtil.GetLast<GroupEntity>().Id;
        testDataUtil.SaveChanges();
    }

    [Given("^that the character is a member of the group$")]
    public void GivenThatTheCharacterIsAMemberOfTheGroup()
    {
        var character = testDataUtil.GetLast<CharacterEntity>();
        character.GroupId = testDataUtil.GetLast<GroupEntity>().Id;
        testDataUtil.SaveChanges();
    }

    [Given("^a character history entry$")]
    public void GivenACharacterHistoryEntry()
    {
        testDataUtil.AddCharacterHistoryEntry();
    }
}