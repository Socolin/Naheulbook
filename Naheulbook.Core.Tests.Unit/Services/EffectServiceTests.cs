using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
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
    public class EffectServiceTests
    {
        private IEffectRepository _effectRepository;
        private IEffectSubCategoryRepository _effectSubCategoryRepository;
        private IEffectTypeRepository _effectTypeRepository;
        private IAuthorizationUtil _authorizationUtil;
        private EffectService _effectService;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void SetUp()
        {
            var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWorkFactory.CreateUnitOfWork().Returns(_unitOfWork);
            _effectRepository = Substitute.For<IEffectRepository>();
            _unitOfWork.Effects.Returns(_effectRepository);
            _effectSubCategoryRepository = Substitute.For<IEffectSubCategoryRepository>();
            _unitOfWork.EffectSubCategories.Returns(_effectSubCategoryRepository);
            _effectTypeRepository = Substitute.For<IEffectTypeRepository>();
            _unitOfWork.EffectTypes.Returns(_effectTypeRepository);
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _effectService = new EffectService(unitOfWorkFactory, _authorizationUtil);
        }

        [Test]
        public async Task GetEffect_LoadEffectFromDatabase()
        {
            var expectedEffect = new EffectEntity();

            _effectRepository.GetWithModifiersAsync(42)
                .Returns(expectedEffect);

            var effect = await _effectService.GetEffectAsync(42);

            effect.Should().BeSameAs(expectedEffect);
        }

        [Test]
        public async Task GetEffectSubCategories()
        {
            var expectedEffectSubCategories = new List<EffectType>();

            _effectRepository.GetCategoriesAsync()
                .Returns(expectedEffectSubCategories);

            var effectSubCategories = await _effectService.GetEffectSubCategoriesAsync();

            effectSubCategories.Should().BeSameAs(expectedEffectSubCategories);
        }

        [Test]
        public async Task CanGetEffectsBySubCategory()
        {
            var expectedEffects = new List<EffectEntity>();

            _effectRepository.GetBySubCategoryWithModifiersAsync(42)
                .Returns(expectedEffects);

            var effects = await _effectService.GetEffectsBySubCategoryAsync(42);

            effects.Should().BeSameAs(expectedEffects);
        }

        [Test]
        public async Task CreateEffectType_AddANewEffectTypeInDatabase()
        {
            var createEffectTypeRequest = AutoFill<CreateEffectTypeRequest>.One();
            var effectType = await _effectService.CreateEffectTypeAsync(new NaheulbookExecutionContext(), createEffectTypeRequest);

            Received.InOrder(() =>
            {
                _effectTypeRepository.Add(effectType);
                _unitOfWork.SaveChangesAsync();
            });
            effectType.Name.Should().Be("some-name");
        }

        [Test]
        public async Task CreateEffectType_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();

            await _effectService.CreateEffectTypeAsync(executionContext, new CreateEffectTypeRequest());

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.SaveChangesAsync();
            });
        }

        [Test]
        public async Task CreateEffectSubCategory_AddANewEffectSubCategoryInDatabase()
        {
            var expectedEffectSubCategory = CreateEffectSubCategory();
            expectedEffectSubCategory.Effects = new List<EffectEntity>();
            var createEffectSubCategoryRequest = AutoFill<CreateEffectSubCategoryRequest>.One();

            var effectSubCategory = await _effectService.CreateEffectSubCategoryAsync(new NaheulbookExecutionContext(), createEffectSubCategoryRequest);

            Received.InOrder(() =>
            {
                _effectSubCategoryRepository.Add(effectSubCategory);
                _unitOfWork.SaveChangesAsync();
            });
            effectSubCategory.Should().BeEquivalentTo(expectedEffectSubCategory);
        }

        [Test]
        public async Task CreateEffectSubCategory_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();
            var createEffectSubCategoryRequest = AutoFill<CreateEffectSubCategoryRequest>.One();

            await _effectService.CreateEffectSubCategoryAsync(executionContext, createEffectSubCategoryRequest);

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.SaveChangesAsync();
            });
        }

        [Test]
        public async Task CreateEffect_AddANewEffectInDatabase()
        {
            var expectedEffect = CreateEffect(subCategoryId: 2);
            var createEffectRequest = AutoFill<CreateEffectRequest>.One();
            var executionContext = new NaheulbookExecutionContext();

            var effect = await _effectService.CreateEffectAsync(executionContext, 2, createEffectRequest);

            Received.InOrder(() =>
            {
                _effectRepository.Add(effect);
                _unitOfWork.SaveChangesAsync();
            });
            effect.Should().BeEquivalentTo(expectedEffect);
        }

        [Test]
        public async Task CreateEffect_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();
            var createEffectRequest = AutoFill<CreateEffectRequest>.One();

            await _effectService.CreateEffectAsync(executionContext, 2, createEffectRequest);

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.SaveChangesAsync();
            });
        }

        [Test]
        public async Task EditEffect_UpdateEffectInDatabase()
        {
            var expectedEffect = CreateEffect(42, subCategoryId: 1, offset: 1);
            var executionContext = new NaheulbookExecutionContext();
            var previousEffect = AutoFill<EffectEntity>.One(AutoFillFlags.RandomizeString | AutoFillFlags.RandomInt, new AutoFillSettings {MaxDepth = 1}, (i) => new {Category = i.SubCategory});
            var editEffectRequest = AutoFill<EditEffectRequest>.One();

            previousEffect.Id = 42;

            _effectRepository.GetWithModifiersAsync(42)
                .Returns(previousEffect);

            await _effectService.EditEffectAsync(executionContext, 42, editEffectRequest);

            await _unitOfWork.Received(1)
                .SaveChangesAsync();
            previousEffect.Should().BeEquivalentTo(expectedEffect);
        }

        [Test]
        public async Task EditEffect_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();
            var previousEffect = AutoFill<EffectEntity>.One(AutoFillFlags.RandomizeString | AutoFillFlags.RandomInt);
            var editEffectRequest = AutoFill<EditEffectRequest>.One();
            previousEffect.Id = 42;

            _effectRepository.GetWithModifiersAsync(42)
                .Returns(previousEffect);

            await _effectService.EditEffectAsync(executionContext, 42, editEffectRequest);

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.SaveChangesAsync();
            });
        }

        [Test]
        public async Task EditEffect_WhenEffectDoesNotExists_Throw()
        {
            var executionContext = new NaheulbookExecutionContext();
            var editEffectRequest = AutoFill<EditEffectRequest>.One();

            _effectRepository.GetWithModifiersAsync(Arg.Any<int>())
                .Returns((EffectEntity) null);

            Func<Task> act = () => _effectService.EditEffectAsync(executionContext, 42, editEffectRequest);

            await act.Should().ThrowAsync<EffectNotFoundException>();
        }

        private static EffectSubCategory CreateEffectSubCategory()
        {
            return new EffectSubCategory
            {
                Name = "some-name",
                Note = "some-note",
                DiceCount = 1,
                DiceSize = 2,
                TypeId = 3,
                Effects = new List<EffectEntity>()
            };
        }

        private static EffectEntity CreateEffect(int id = 0, int offset = 0, int subCategoryId = 0)
        {
            return new EffectEntity
            {
                Id = id,
                Name = "some-name",
                Description = "some-description",
                DurationType = "some-duration-type",
                Duration = "some-duration",
                CombatCount = 1 + offset,
                Dice = (short?) (2 + offset),
                LapCount = 3 + offset,
                TimeDuration = 4 + offset,
                Modifiers = new List<EffectModifier>
                {
                    new EffectModifier
                    {
                        StatName = "some-stat",
                        Type = "some-type",
                        Value = (short) (5 + offset)
                    },
                    new EffectModifier
                    {
                        StatName = "some-stat",
                        Type = "some-type",
                        Value = (short) (6 + offset)
                    },
                    new EffectModifier
                    {
                        StatName = "some-stat",
                        Type = "some-type",
                        Value = (short) (7 + offset)
                    }
                },
                SubCategoryId = subCategoryId
            };
        }
    }
}