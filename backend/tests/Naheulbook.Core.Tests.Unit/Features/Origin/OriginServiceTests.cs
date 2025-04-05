using FluentAssertions;
using Naheulbook.Core.Features.Origin;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Origin;

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