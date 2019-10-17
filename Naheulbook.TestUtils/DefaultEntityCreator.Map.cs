using Naheulbook.Data.Models;
using Newtonsoft.Json;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Map CreateMap(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Map
            {
                Name = $"some-map-name-{suffix}",
                Data = JsonConvert.SerializeObject(new
                {
                    attribution = new[]
                    {
                        new
                        {
                            name = "some-attribution-name",
                            url = "some-attribution-url"
                        }
                    }
                })
            };
        }
    }
}