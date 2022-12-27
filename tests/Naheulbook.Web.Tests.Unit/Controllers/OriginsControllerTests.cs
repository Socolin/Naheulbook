using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class OriginsControllerTests
{
    private IOriginService _originService;
    private IMapper _mapper;
    private ICharacterRandomNameService _characterRandomNameService;

    private OriginsController _originsController;

    [SetUp]
    public void SetUp()
    {
        _originService = Substitute.For<IOriginService>();
        _mapper = Substitute.For<IMapper>();
        _characterRandomNameService = Substitute.For<ICharacterRandomNameService>();

        _originsController = new OriginsController(_originService, _mapper, _characterRandomNameService);
    }

    [Test]
    public async Task CanGetOrigins()
    {
        var origins = new List<OriginEntity>();
        var expectedResponse = new List<OriginResponse>();

        _originService.GetOriginsAsync()
            .Returns(origins);
        _mapper.Map<List<OriginResponse>>(origins)
            .Returns(expectedResponse);

        var result = await _originsController.GetAsync();

        result.Value.Should().BeSameAs(expectedResponse);
    }
}