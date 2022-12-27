using AutoMapper;
using Naheulbook.Web.Mappers;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Mappers;

public class AutoMapperTests
{
    [Test]
    public void MapperProfileIsValid()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile<MapperProfile>(); });

        mapperConfiguration.AssertConfigurationIsValid();
    }
}