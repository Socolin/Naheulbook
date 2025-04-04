using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Extensions;

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
            throw new HttpErrorException(StatusCodes.Status401Unauthorized, "Not authenticated");
        }

        return (NaheulbookExecutionContext) context.Items[NaheulbookExecutionContextKey]!;
    }

    public static OptionalNaheulbookExecutionContext GetIfExistsExecutionContext(this HttpContext context)
    {
        if (!context.Items.ContainsKey(NaheulbookExecutionContextKey))
            return new OptionalNaheulbookExecutionContext();

        return new OptionalNaheulbookExecutionContext((NaheulbookExecutionContext?) context.Items[NaheulbookExecutionContextKey]);
    }
}