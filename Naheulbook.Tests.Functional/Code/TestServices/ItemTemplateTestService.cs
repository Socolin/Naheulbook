using System.Threading.Tasks;
using Naheulbook.Requests.Requests;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Web.Responses;

namespace Naheulbook.Tests.Functional.Code.TestServices
{
    public class ItemTemplateTestService
    {
        private readonly NaheulbookHttpClient _naheulbookHttpClient;

        public ItemTemplateTestService(NaheulbookHttpClient naheulbookHttpClient)
        {
            _naheulbookHttpClient = naheulbookHttpClient;
        }

        public Task<ItemTemplateSectionResponse> CreateEffectTypeAsync(CreateItemTemplateSectionRequest request, string jwt)
        {
            return _naheulbookHttpClient.PostAndParseJsonResultAsync<ItemTemplateSectionResponse>("/api/v2/itemTemplateSections", request, jwt);
        }

        public Task<ItemTemplateCategoryResponse> CreateEffectCategoryAsync(CreateItemTemplateCategoryRequest request, string jwt)
        {
            return _naheulbookHttpClient.PostAndParseJsonResultAsync<ItemTemplateCategoryResponse>("/api/v2/itemTemplateCategories", request, jwt);
        }
    }
}