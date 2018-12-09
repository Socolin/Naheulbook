using System.Threading.Tasks;
using Naheulbook.Requests.Requests;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Web.Responses;

namespace Naheulbook.Tests.Functional.Code.TestServices
{
    public class EffectTestService
    {
        private readonly NaheulbookHttpClient _naheulbookHttpClient;

        public EffectTestService(NaheulbookHttpClient naheulbookHttpClient)
        {
            _naheulbookHttpClient = naheulbookHttpClient;
        }

        public Task<EffectTypeResponse> CreateEffectTypeAsync(CreateEffectTypeRequest request, string jwt)
        {
            return _naheulbookHttpClient.PostAndParseJsonResultAsync<EffectTypeResponse>("/api/v2/effectTypes", request, jwt);
        }

        public Task<EffectCategoryResponse> CreateEffectCategoryAsync(CreateEffectCategoryRequest request, string jwt)
        {
            return _naheulbookHttpClient.PostAndParseJsonResultAsync<EffectCategoryResponse>("/api/v2/effectCategories", request, jwt);
        }

        public Task<EffectResponse> CreateEffectAsync(CreateEffectRequest request, string jwt)
        {
            return _naheulbookHttpClient.PostAndParseJsonResultAsync<EffectResponse>("/api/v2/effects", request, jwt);
        }
    }
}