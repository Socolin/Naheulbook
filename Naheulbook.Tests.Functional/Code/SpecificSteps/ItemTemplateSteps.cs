using Naheulbook.Requests.Requests;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.TestServices;
using Socolin.TestUtils.AutoFillTestObjects;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class ItemTemplateSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ItemTemplateTestService _itemTemplateTestService;

        public ItemTemplateSteps(ScenarioContext scenarioContext, ItemTemplateTestService itemTemplateTestService)
        {
            _scenarioContext = scenarioContext;
            _itemTemplateTestService = itemTemplateTestService;
        }

        [Given("an item template section")]
        public void GivenAnItemTemplateSection()
        {
            var createItemTemplateSectionRequest = AutoFill<CreateItemTemplateSectionRequest>.One(AutoFillFlags.RandomizeString);
            var itemTemplateSection = _itemTemplateTestService.CreateEffectTypeAsync(createItemTemplateSectionRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetItemTemplateSectionId(itemTemplateSection.Id);
        }
    }
}