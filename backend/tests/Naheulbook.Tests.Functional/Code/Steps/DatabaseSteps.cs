using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.Steps;

/// <summary>
/// Check the state of the database, for changes not visible through the API (e.g. data of other users)
/// </summary>
[Binding]
public class DatabaseSteps(TestDataUtil testDataUtil)
{
    [Then("^the (.+) (user|group|character|item template) (does not exist anymore|still exists)$")]
    public async Task ThenTheXEntityExistsOrNot(string indexString, string entityName, string existence)
    {
        var index = StepArgumentUtil.ParseIndex(indexString);
        await using var dbContext = testDataUtil.CreateDbContext();

        var exists = entityName switch
        {
            "user" => await dbContext.Users.AnyAsync(e => e.Id == testDataUtil.Get<UserEntity>(index).Id),
            "group" => await dbContext.Groups.AnyAsync(e => e.Id == testDataUtil.Get<GroupEntity>(index).Id),
            "character" => await dbContext.Characters.AnyAsync(e => e.Id == testDataUtil.Get<CharacterEntity>(index).Id),
            "item template" => await dbContext.ItemTemplates.AnyAsync(e => e.Id == testDataUtil.Get<ItemTemplateEntity>(index).Id),
            _ => throw new NotSupportedException(entityName),
        };

        exists.Should().Be(existence == "still exists", $"the {indexString} {entityName} {existence}");
    }

    [Then("^the (.+) character is not in a group anymore$")]
    public async Task ThenTheXCharacterIsNotInAGroupAnymore(string indexString)
    {
        var characterId = testDataUtil.Get<CharacterEntity>(StepArgumentUtil.ParseIndex(indexString)).Id;
        await using var dbContext = testDataUtil.CreateDbContext();

        var character = await dbContext.Characters.SingleAsync(c => c.Id == characterId);

        character.GroupId.Should().BeNull();
    }

    [Then("^the (.+) item template has no owner anymore$")]
    public async Task ThenTheXItemTemplateHasNoOwnerAnymore(string indexString)
    {
        var itemTemplateId = testDataUtil.Get<ItemTemplateEntity>(StepArgumentUtil.ParseIndex(indexString)).Id;
        await using var dbContext = testDataUtil.CreateDbContext();

        var itemTemplate = await dbContext.ItemTemplates.SingleAsync(i => i.Id == itemTemplateId);

        itemTemplate.SourceUserId.Should().BeNull();
    }
}
