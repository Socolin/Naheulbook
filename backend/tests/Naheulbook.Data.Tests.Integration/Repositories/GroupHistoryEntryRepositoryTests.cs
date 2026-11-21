using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class GroupHistoryEntryRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private static readonly DateTimeOffset NowUtc = new(2025, 03, 28, 10, 12, 8, TimeSpan.Zero);
    private GroupHistoryEntryRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new GroupHistoryEntryRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetByGroupIdAndPageAsync_ReturnsMostRecentEntriesFirst()
    {
        TestDataUtil
            .AddUser()
            .AddGroup(out var group)
            .AddGroupHistoryEntry(out var entry1, h => h.Date = NowUtc.AddMinutes(100).UtcDateTime)
            .AddGroupHistoryEntry(out var entry3, h => h.Date = NowUtc.AddMinutes(300).UtcDateTime)
            .AddGroupHistoryEntry(out var entry2, h => h.Date = NowUtc.AddMinutes(200).UtcDateTime)
            ;

        var actual = await _repository.GetByGroupIdAndPageAsync(group.Id, 0);

        AssertEntitiesAreLoadedWithSameOrder(actual, [entry3, entry2, entry1]);
    }
}
