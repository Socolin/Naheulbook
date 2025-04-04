using System;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddMap(Action<MapEntity> customizer = null)
    {
        var map = new MapEntity
        {
            Name = RngUtil.GetRandomString("some-map-name"),
            ImageData = JsonConvert.SerializeObject(new MapImageData
                {
                    Width = 40,
                    Height = 20,
                    ZoomCount = 5,
                    ExtraZoomCount = 1,
                },
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                }
            ),
            Data = JsonConvert.SerializeObject(new MapData
                {
                    IsGm = true,
                    Attribution =
                    [
                        new MapData.MapAttribution
                        {
                            Name = "some-attribution-name",
                            Url = "some-attribution-url",
                        },
                    ],
                },
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                }
            ),
        };

        return SaveEntity(map, customizer);
    }
}