using System.Threading.Tasks;
using Naheulbook.Requests.Requests;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Web.Responses;

namespace Naheulbook.Tests.Functional.Code.TestServices;

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

    public Task<EffectSubCategoryResponse> CreateEffectSubCategoryAsync(CreateEffectSubCategoryRequest request, string jwt)
    {
        return _naheulbookHttpClient.PostAndParseJsonResultAsync<EffectSubCategoryResponse>("/api/v2/effectSubCategories", request, jwt);
    }

    public Task<EffectResponse> CreateEffectAsync(CreateEffectRequest request, string jwt)
    {
        return _naheulbookHttpClient.PostAndParseJsonResultAsync<EffectResponse>("/api/v2/effects", request, jwt);
    }
}