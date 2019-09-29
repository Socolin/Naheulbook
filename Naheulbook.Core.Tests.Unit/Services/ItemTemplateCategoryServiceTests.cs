using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestUtils.AutoFillTestObjects;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class ItemTemplateCategoryServiceTests
    {
        private IItemTemplateCategoryRepository _itemTemplateCategoryRepository;
        private IUnitOfWork _unitOfWork;
        private IAuthorizationUtil _authorizationUtil;
        private ItemTemplateCategoryService _itemTemplateCategoryService;

        [SetUp]
        public void SetUp()
        {
            var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWorkFactory.CreateUnitOfWork().Returns(_unitOfWork);
            _itemTemplateCategoryRepository = Substitute.For<IItemTemplateCategoryRepository>();
            _unitOfWork.ItemTemplateCategories.Returns(_itemTemplateCategoryRepository);
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _itemTemplateCategoryService = new ItemTemplateCategoryService(unitOfWorkFactory, _authorizationUtil);
        }

        [Test]
        public async Task CreateItemTemplateCategory_AddANewItemTemplateCategoryInDatabase()
        {
            var expectedItemTemplateCategory = CreateItemTemplateCategory();
            var createItemTemplateCategoryRequest = AutoFill<CreateItemTemplateCategoryRequest>.One(settings: new AutoFillSettings {MaxDepth = 0});

            var itemTemplateCategory = await _itemTemplateCategoryService.CreateItemTemplateCategoryAsync(new NaheulbookExecutionContext(), createItemTemplateCategoryRequest);

            Received.InOrder(() =>
            {
                _itemTemplateCategoryRepository.Add(itemTemplateCategory);
                _unitOfWork.SaveChangesAsync();
            });
            itemTemplateCategory.Should().BeEquivalentTo(expectedItemTemplateCategory);
        }

        [Test]
        public async Task CreateItemTemplateCategory_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();

            await _itemTemplateCategoryService.CreateItemTemplateCategoryAsync(executionContext, new CreateItemTemplateCategoryRequest());

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.SaveChangesAsync();
            });
        }

        private ItemTemplateCategory CreateItemTemplateCategory()
        {
            return new ItemTemplateCategory
            {
                Name = "some-name",
                TechName= "some-tech-name",
                Description = "some-description",
                Note = "some-note",
                SectionId = 1
            };
        }
    }
}