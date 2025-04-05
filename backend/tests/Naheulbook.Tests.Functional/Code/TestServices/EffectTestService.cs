using Naheulbook.Requests.Requests;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Web.Responses;

namespace Naheulbook.Tests.Functional.Code.TestServices;

public class EffectTestService(NaheulbookHttpClient naheulbookHttpClient)
{
    public Task<EffectTypeResponse> CreateEffectTypeAsync(CreateEffectTypeRequest request, string jwt)
    {
        return naheulbookHttpClient.PostAndParseJsonResultAsync<EffectTypeResponse>("/api/v2/effectTypes", request, jwt);
    }

    public Task<EffectSubCategoryResponse> CreateEffectSubCategoryAsync(CreateEffectSubCategoryRequest request, string jwt)
    {
        return naheulbookHttpClient.PostAndParseJsonResultAsync<EffectSubCategoryResponse>("/api/v2/effectSubCategories", request, jwt);
    }

    public Task<EffectResponse> CreateEffectAsync(CreateEffectRequest request, string jwt)
    {
        return naheulbookHttpClient.PostAndParseJsonResultAsync<EffectResponse>("/api/v2/effects", request, jwt);
    }
}