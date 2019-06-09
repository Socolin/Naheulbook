using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class ItemTemplateRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private ItemTemplateRepository _itemTemplateRepository;

        [SetUp]
        public void SetUp()
        {
            TestDataUtil.Cleanup();
            _itemTemplateRepository = new ItemTemplateRepository(RepositoryDbContext);
        }

        [Test]
        public async Task GetByIdsAsync_LoadAllEntitiesMatchingGivenIds()
        {
            TestDataUtil
                .AddItemTemplateSection()
                .AddItemTemplateCategory()
                .AddItemTemplate()
                .AddItemTemplate()
                .AddItemTemplate();

            var itemTemplateIds = TestDataUtil.GetAll<ItemTemplate>().Select(x => x.Id);

            var actual = await _itemTemplateRepository.GetByIdsAsync(itemTemplateIds);

            actual.Should().BeEquivalentTo(TestDataUtil.GetAll<ItemTemplate>(), config => config.Excluding(x => x.Category));
        }
    }
}