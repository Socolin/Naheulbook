using System.Net;
using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Models;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Extensions
{
    public static class HttpContextExtension
    {
        private const string NaheulbookExecutionContextKey = "NaheulbookExecutionContext";

        public static void SetExecutionContext(this HttpContext context, NaheulbookExecutionContext executionContext)
        {
            context.Items[NaheulbookExecutionContextKey] = executionContext;
        }

        public static NaheulbookExecutionContext GetExecutionContext(this HttpContext context)
        {
            if (!context.Items.ContainsKey(NaheulbookExecutionContextKey))
            {
                throw new HttpErrorException(HttpStatusCode.Unauthorized, "Not authenticated");
            }

            return (NaheulbookExecutionContext) context.Items[NaheulbookExecutionContextKey];
        }

        public static IOptionalNaheulbookExecutionContext GetIfExistsExecutionContext(this HttpContext context)
        {
            if (!context.Items.ContainsKey(NaheulbookExecutionContextKey))
                return null;

            return (NaheulbookExecutionContext) context.Items[NaheulbookExecutionContextKey];
        }
    }
}