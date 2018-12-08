using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Models;

namespace Naheulbook.Web.Extensions
{
    public static class HttpContextExtension
    {
        public static void SetExecutionContext(this HttpContext context, NaheulbookExecutionContext executionContext)
        {
            context.Items["NaheulbookExecutionContext"] = executionContext;
        }

        public static NaheulbookExecutionContext GetExecutionContext(this HttpContext context)
        {
            return (NaheulbookExecutionContext) context.Items["NaheulbookExecutionContext"];
        }
    }
}