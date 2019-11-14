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
    public class ItemTemplateSubCategoryServiceTests
    {
        private IItemTemplateSubCategoryRepository _itemTemplateSubCategoryRepository;
        private IUnitOfWork _unitOfWork;
        private IAuthorizationUtil _authorizationUtil;
        private ItemTemplateSubCategoryService _itemTemplateSubCategoryService;

        [SetUp]
        public void SetUp()
        {
            var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWorkFactory.CreateUnitOfWork().Returns(_unitOfWork);
            _itemTemplateSubCategoryRepository = Substitute.For<IItemTemplateSubCategoryRepository>();
            _unitOfWork.ItemTemplateSubCategories.Returns(_itemTemplateSubCategoryRepository);
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _itemTemplateSubCategoryService = new ItemTemplateSubCategoryService(unitOfWorkFactory, _authorizationUtil);
        }

        [Test]
        public async Task CreateItemTemplateSubCategory_AddANewItemTemplateSubCategoryInDatabase()
        {
            var expectedItemTemplateSubCategory = CreateItemTemplateSubCategoryAsync();
            var createItemTemplateSubCategoryRequest = AutoFill<CreateItemTemplateSubCategoryRequest>.One(settings: new AutoFillSettings {MaxDepth = 0});

            var itemTemplateSubCategory = await _itemTemplateSubCategoryService.CreateItemTemplateSubCategoryAsync(new NaheulbookExecutionContext(), createItemTemplateSubCategoryRequest);

            Received.InOrder(() =>
            {
                _itemTemplateSubCategoryRepository.Add(itemTemplateSubCategory);
                _unitOfWork.SaveChangesAsync();
            });
            itemTemplateSubCategory.Should().BeEquivalentTo(expectedItemTemplateSubCategory);
        }

        [Test]
        public async Task CreateItemTemplateSubCategoryAsync_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();

            await _itemTemplateSubCategoryService.CreateItemTemplateSubCategoryAsync(executionContext, new CreateItemTemplateSubCategoryRequest());

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.SaveChangesAsync();
            });
        }

        private ItemTemplateSubCategory CreateItemTemplateSubCategoryAsync()
        {
            return new ItemTemplateSubCategory
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