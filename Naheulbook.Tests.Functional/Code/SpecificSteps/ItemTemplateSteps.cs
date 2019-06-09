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
        private readonly TestDataUtil _testDataUtil;

        public ItemTemplateSteps(
            TestDataUtil testDataUtil
        )
        {
            _testDataUtil = testDataUtil;
        }

        [Given("an item slot")]
        public void GivenAnItemSlot()
        {
            _testDataUtil.AddSlot();
        }

        [Given("an item template")]
        public void GivenAnItemTemplate()
        {
            _testDataUtil.AddItemTemplate();
        }

        [Given("an item template section")]
        public void GivenAnItemTemplateSection()
        {
            _testDataUtil.AddItemTemplateSection();
        }

        [Given("an item template category")]
        public void GivenAnItemTemplateCategory()
        {
            _testDataUtil.AddItemTemplateCategory();
        }
    }
}