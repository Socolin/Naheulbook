using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class EffectServiceTests
    {
        private IEffectRepository _effectRepository;
        private EffectService _effectService;

        [SetUp]
        public void SetUp()
        {
            var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            var unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);
            _effectRepository = Substitute.For<IEffectRepository>();
            unitOfWork.Effects.Returns(_effectRepository);
            _effectService = new EffectService(unitOfWorkFactory);
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
    }
}