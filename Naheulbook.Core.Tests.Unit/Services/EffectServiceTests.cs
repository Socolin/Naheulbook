using System.Collections.Generic;
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
    public class EffectServiceTests
    {
        private IEffectRepository _effectRepository;
        private IEffectCategoryRepository _effectCategoryRepository;
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
            _effectCategoryRepository = Substitute.For<IEffectCategoryRepository>();
            _unitOfWork.EffectCategories.Returns(_effectCategoryRepository);
            _effectTypeRepository = Substitute.For<IEffectTypeRepository>();
            _unitOfWork.EffectTypes.Returns(_effectTypeRepository);
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _effectService = new EffectService(unitOfWorkFactory, _authorizationUtil);
        }

        [Test]
        public async Task GetEffectCategories()
        {
            var expectedEffectCategories = new List<EffectType>();

            _effectRepository.GetCategoriesAsync()
                .Returns(expectedEffectCategories);

            var effectCategories = await _effectService.GetEffectCategoriesAsync();

            effectCategories.Should().BeSameAs(expectedEffectCategories);
        }

        [Test]
        public async Task CanGetEffectsByCategory()
        {
            var expectedEffects = new List<Effect>();

            _effectRepository.GetByCategoryWithModifiersAsync(42)
                .Returns(expectedEffects);

            var effects = await _effectService.GetEffectsByCategoryAsync(42);

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
                _unitOfWork.CompleteAsync();
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
                _unitOfWork.CompleteAsync();
            });
        }

        [Test]
        public async Task CreateEffectCategory_AddANewEffectCategoryInDatabase()
        {
            var expectedEffectCategory = CreateEffectCategory();
            expectedEffectCategory.Effects = new List<Effect>();
            var createEffectCategoryRequest = AutoFill<CreateEffectCategoryRequest>.One();

            var effectCategory = await _effectService.CreateEffectCategoryAsync(new NaheulbookExecutionContext(), createEffectCategoryRequest);

            Received.InOrder(() =>
            {
                _effectCategoryRepository.Add(effectCategory);
                _unitOfWork.CompleteAsync();
            });
            effectCategory.Should().BeEquivalentTo(expectedEffectCategory);
        }

        [Test]
        public async Task CreateEffectCategory_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();
            var createEffectCategoryRequest = AutoFill<CreateEffectCategoryRequest>.One();

            await _effectService.CreateEffectCategoryAsync(executionContext, createEffectCategoryRequest);

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.CompleteAsync();
            });
        }

        [Test]
        public async Task CreateEffect_AddANewEffectInDatabase()
        {
            var expectedEffect = CreateEffect();
            var createEffectRequest = AutoFill<CreateEffectRequest>.One();
            var executionContext = new NaheulbookExecutionContext();

            var effect = await _effectService.CreateEffectAsync(executionContext, createEffectRequest);

            Received.InOrder(() =>
            {
                _effectRepository.Add(effect);
                _unitOfWork.CompleteAsync();
            });
            effect.Should().BeEquivalentTo(expectedEffect);
        }

        [Test]
        public async Task CreateEffect_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();
            var createEffectRequest = AutoFill<CreateEffectRequest>.One();

            await _effectService.CreateEffectAsync(executionContext, createEffectRequest);

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.CompleteAsync();
            });
        }

        private static EffectCategory CreateEffectCategory()
        {
            return new EffectCategory
            {
                Name = "some-name",
                Note = "some-note",
                DiceCount = 1,
                DiceSize = 2,
                TypeId = 3
            };
        }

        private static Effect CreateEffect()
        {
            return new Effect
            {
                Name = "some-name",
                Description = "some-description",
                DurationType = "some-duration-type",
                Duration = "some-duration",
                CombatCount = 1,
                CategoryId = 2,
                Dice = 3,
                LapCount = 4,
                TimeDuration = 5,
                Modifiers = new List<EffectModifier>
                {
                    new EffectModifier
                    {
                        StatName = "some-stat",
                        Type = "some-type",
                        Value = 6
                    },
                    new EffectModifier
                    {
                        StatName = "some-stat",
                        Type = "some-type",
                        Value = 7
                    },
                    new EffectModifier
                    {
                        StatName = "some-stat",
                        Type = "some-type",
                        Value = 8
                    }
                },
            };
        }
    }
}