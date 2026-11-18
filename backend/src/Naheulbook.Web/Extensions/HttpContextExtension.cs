using Naheulbook.Core.Features.Shared;
using Naheulbook.Shared.Extensions;
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
        if (context.Items.TryGetValue(NaheulbookExecutionContextKey, out var item))
            return (NaheulbookExecutionContext)item.NotNull();

        throw new HttpErrorException(StatusCodes.Status401Unauthorized, "Not authenticated");
    }

    public static OptionalNaheulbookExecutionContext GetIfExistsExecutionContext(this HttpContext context)
    {
        if (context.Items.TryGetValue(NaheulbookExecutionContextKey, out var item))
            return new OptionalNaheulbookExecutionContext((NaheulbookExecutionContext?)item);

        return new OptionalNaheulbookExecutionContext();
    }
}