using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddMap(Action<MapEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateMap(), customizer);
    }
}