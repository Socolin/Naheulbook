using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.TestServices;
using Naheulbook.TestUtils;
using Socolin.TestUtils.AutoFillTestObjects;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class ItemTemplateSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ItemTemplateTestService _itemTemplateTestService;
        private readonly DbContextOptions<NaheulbookDbContext> _dbContextOptions;
        private readonly TestDataUtil _testDataUtil;

        public ItemTemplateSteps(
            ScenarioContext scenarioContext,
            ItemTemplateTestService itemTemplateTestService,
            DbContextOptions<NaheulbookDbContext> dbContextOptions,
            TestDataUtil testDataUtil
        )
        {
            _scenarioContext = scenarioContext;
            _itemTemplateTestService = itemTemplateTestService;
            _dbContextOptions = dbContextOptions;
            _testDataUtil = testDataUtil;
        }

        [Given("an item slot")]
        public async Task GivenAnItemSlot()
        {
            using (var dbContext = new NaheulbookDbContext(_dbContextOptions))
            {
                var itemSlot = new Slot
                {
                    Name = $"some-slot-name-{RngUtils.GetRandomHexString(4)}",
                    TechName = $"SOME_SLOT_TECH_NAME_{RngUtils.GetRandomHexString(4)}"
                };
                dbContext.Slots.Add(itemSlot);
                await dbContext.SaveChangesAsync();
                _scenarioContext.SetItemSlot(itemSlot);
            }
        }

        [Given("an item template")]
        public void GivenAnItemTemplate()
        {
            // TODO: Move 2 first to following methods
            _testDataUtil
                .AddItemTemplateSection()
                .AddItemTemplateCategory()
                .AddItemTemplate();
        }

        [Given("an item template section")]
        public void GivenAnItemTemplateSection()
        {
            var createItemTemplateSectionRequest = AutoFill<CreateItemTemplateSectionRequest>.One(AutoFillFlags.RandomizeString);
            var itemTemplateSection = _itemTemplateTestService.CreateEffectTypeAsync(createItemTemplateSectionRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetItemTemplateSectionId(itemTemplateSection.Id);
        }

        [Given("an item template category")]
        public void GivenAnItemTemplateCategory()
        {
            GivenAnItemTemplateSection();

            var createItemTemplateCategoryRequest = AutoFill<CreateItemTemplateCategoryRequest>.One(AutoFillFlags.RandomizeString);
            createItemTemplateCategoryRequest.SectionId = _scenarioContext.GetItemTemplateSectionId();
            var itemTemplateCategory = _itemTemplateTestService.CreateEffectCategoryAsync(createItemTemplateCategoryRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetItemTemplateCategoryId(itemTemplateCategory.Id);
        }
    }
}