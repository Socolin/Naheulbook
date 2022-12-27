using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Extensions.UnitOfWorks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services;

public class SkillServiceTests
{
    private ISkillRepository _skillRepository;
    private SkillService _skillService;

    [SetUp]
    public void SetUp()
    {
        var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);
        _skillRepository = Substitute.For<ISkillRepository>();
        unitOfWork.Skills.Returns(_skillRepository);
        _skillService = new SkillService(unitOfWorkFactory);
    }

    [Test]
    public async Task CanListAllSkills()
    {
        var expectedSkills = new List<SkillEntity>();

        _skillRepository.GetAllWithEffectsAsync()
            .Returns(expectedSkills);

        var actualSkills = await _skillService.GetSkillsAsync();

        actualSkills.Should().BeSameAs(expectedSkills);
    }
}