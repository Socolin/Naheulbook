using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddAptitudeGroup(Action<AptitudeGroupEntity> customizer = null)
    {
        return AddAptitudeGroup(out _, customizer);
    }

    public TestDataUtil AddAptitudeGroup(out AptitudeGroupEntity aptitudeGroup, Action<AptitudeGroupEntity> customizer = null)
    {
        aptitudeGroup = new AptitudeGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = RngUtil.GetRandomString("some-aptitude-group-name"),
            Aptitudes = [],
        };

        return SaveEntity(aptitudeGroup, customizer);
    }

    public TestDataUtil AddAptitude(Action<AptitudeEntity> customizer = null)
    {
        return AddAptitude(out _, customizer);
    }

    public TestDataUtil AddAptitude(out AptitudeEntity aptitude, Action<AptitudeEntity> customizer = null)
    {
        var aptitudeGroup = GetLast<AptitudeGroupEntity>();
        aptitude = new AptitudeEntity
        {
            Id = Guid.NewGuid(),
            Name = RngUtil.GetRandomString("some-aptitude-name"),
            Roll = Random.Shared.Next(1, 101),
            AptitudeGroupId = aptitudeGroup.Id,
            AptitudeGroup = aptitudeGroup,
            Description = RngUtil.GetRandomString("some-aptitude-description"),
            Effect = RngUtil.GetRandomString("some-aptitude-effect"),
            Type = RngUtil.GetRandomString("some-aptitude-type"),
        };

        return SaveEntity(aptitude, customizer);
    }
}