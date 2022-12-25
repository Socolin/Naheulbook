using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public MapEntity CreateMap(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new MapEntity
            {
                Name = $"some-map-name-{suffix}",
                ImageData = JsonConvert.SerializeObject(new MapImageData
                {
                    Width = 40,
                    Height = 20,
                    ZoomCount = 5,
                    ExtraZoomCount = 1
                }, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
                Data = JsonConvert.SerializeObject(new MapData
                {
                    IsGm = true,
                    Attribution = new List<MapData.MapAttribution>()
                    {
                        new MapData.MapAttribution
                        {
                            Name = "some-attribution-name",
                            Url = "some-attribution-url"
                        }
                    }
                }, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })
            };
        }
    }
}