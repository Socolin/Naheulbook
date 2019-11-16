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
            _itemTemplateRepository = new ItemTemplateRepository(RepositoryDbContext);
        }

        [Test]
        public async Task GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync_LoadEntityWithRelatedEntities()
        {
            TestDataUtil.AddItemTemplateWithAllData();

            var itemTemplateId = TestDataUtil.Get<ItemTemplate>().Id;

            var actual = await _itemTemplateRepository.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId);

            actual.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>(), config =>
                config.Excluding(x => x.SubCategory)
                    .Excluding(x => x.Requirements)
                    .Excluding(x => x.Modifiers)
                    .Excluding(x => x.Skills)
                    .Excluding(x => x.UnSkills)
                    .Excluding(x => x.SkillModifiers)
                    .Excluding(x => x.Slots)
                );
            actual.Modifiers.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Modifiers, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.RequireJob).Excluding(x => x.RequiredOrigin).Excluding(x => x.Stat));
            actual.Requirements.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Requirements, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Stat));
            actual.Skills.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Skills, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Skill));
            actual.UnSkills.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().UnSkills, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Skill));
            actual.SkillModifiers.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().SkillModifiers, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Skill));
            actual.Slots.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Slots, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Slot));
            actual.Slots.First().Slot.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Slots.First().Slot);
        }

        [Test]
        public async Task GetByIdsAsync_LoadAllEntitiesMatchingGivenIds()
        {
            TestDataUtil
                .AddItemTemplateSection()
                .AddItemTemplateSubCategory()
                .AddItemTemplate()
                .AddItemTemplate()
                .AddItemTemplate();

            var itemTemplateIds = TestDataUtil.GetAll<ItemTemplate>().Select(x => x.Id);

            var actual = await _itemTemplateRepository.GetByIdsAsync(itemTemplateIds);

            actual.Should().BeEquivalentTo(TestDataUtil.GetAll<ItemTemplate>(), config => config.Excluding(x => x.SubCategory));
        }
    }
}