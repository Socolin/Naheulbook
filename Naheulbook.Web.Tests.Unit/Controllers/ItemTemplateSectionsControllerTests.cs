using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class ItemTemplateSectionsControllerTests : BaseControllerTests
    {
        private IItemTemplateSectionService _itemTemplateSectionService;
        private IMapper _mapper;
        private ItemTemplateSectionsController _itemTemplateSectionsController;

        [SetUp]
        public void SetUp()
        {
            _itemTemplateSectionService = Substitute.For<IItemTemplateSectionService>();
            _mapper = Substitute.For<IMapper>();
            _itemTemplateSectionsController = new ItemTemplateSectionsController(_itemTemplateSectionService, _mapper);
            MockHttpContext(_itemTemplateSectionsController);
        }

        [Test]
        public async Task PostCreateSection_CallItemSectionService()
        {
            var createItemTemplateSectionRequest = new CreateItemTemplateSectionRequest();
            var itemTemplateSection = new ItemTemplateSection();
            var itemTemplateSectionResponse = new ItemTemplateSectionResponse();

            _itemTemplateSectionService.CreateItemTemplateSectionAsync(ExecutionContext, createItemTemplateSectionRequest)
                .Returns(itemTemplateSection);
            _mapper.Map<ItemTemplateSectionResponse>(itemTemplateSection)
                .Returns(itemTemplateSectionResponse);

            var result = await _itemTemplateSectionsController.PostCreateSectionAsync(createItemTemplateSectionRequest);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(itemTemplateSectionResponse);
        }
    }
}