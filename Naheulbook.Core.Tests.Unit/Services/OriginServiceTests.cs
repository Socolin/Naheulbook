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

public class OriginServiceTests
{
    private IOriginRepository _originRepository;
    private OriginService _originService;

    [SetUp]
    public void SetUp()
    {
        var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);
        _originRepository = Substitute.For<IOriginRepository>();
        unitOfWork.Origins.Returns(_originRepository);
        _originService = new OriginService(unitOfWorkFactory);
    }

    [Test]
    public async Task CanGetOrigins()
    {
        var expectedOrigins = new List<OriginEntity>();

        _originRepository.GetAllWithAllDataAsync()
            .Returns(expectedOrigins);

        var origins = await _originService.GetOriginsAsync();

        origins.Should().BeSameAs(expectedOrigins);
    }
}