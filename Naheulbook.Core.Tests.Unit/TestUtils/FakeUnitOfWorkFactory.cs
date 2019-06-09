using System.Collections.Generic;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using NSubstitute;

namespace Naheulbook.Core.Tests.Unit.TestUtils
{
    public class FakeUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly List<IUnitOfWork> _hubUnitOfWorks = new List<IUnitOfWork>();
        private int _uowIndex;

        public FakeUnitOfWorkFactory(int countUnitOfWork = 1)
        {
            AddUnitOfWork(countUnitOfWork);
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return _hubUnitOfWorks[_uowIndex++];
        }

        public IUnitOfWork GetUnitOfWork(int i = 0)
        {
            while (_hubUnitOfWorks.Count <= i)
                _hubUnitOfWorks.Add(CreateSubstituteUnitOfWork());
            return _hubUnitOfWorks[i];
        }

        private void AddUnitOfWork(int count)
        {
            for (var i = 0; i < count; i++)
                _hubUnitOfWorks.Add(CreateSubstituteUnitOfWork());
        }

        private static IUnitOfWork CreateSubstituteUnitOfWork()
        {
            var uow = Substitute.For<IUnitOfWork>();
            uow.Effects.Returns(Substitute.For<IEffectRepository>());
            uow.EffectCategories.Returns(Substitute.For<IEffectCategoryRepository>());
            uow.EffectTypes.Returns(Substitute.For<IEffectTypeRepository>());
            uow.ItemTemplates.Returns(Substitute.For<IItemTemplateRepository>());
            uow.ItemTemplateCategories.Returns(Substitute.For<IItemTemplateCategoryRepository>());
            uow.ItemTemplateSections.Returns(Substitute.For<IItemTemplateSectionRepository>());
            uow.Jobs.Returns(Substitute.For<IJobRepository>());
            uow.MonsterTypes.Returns(Substitute.For<IMonsterTypeRepository>());
            uow.MonsterCategories.Returns(Substitute.For<IMonsterCategoryRepository>());
            uow.MonsterTemplates.Returns(Substitute.For<IMonsterTemplateRepository>());
            uow.Origins.Returns(Substitute.For<IOriginRepository>());
            uow.Skills.Returns(Substitute.For<ISkillRepository>());
            uow.Slots.Returns(Substitute.For<ISlotRepository>());
            uow.Users.Returns(Substitute.For<IUserRepository>());
            return uow;
        }
    }
}