namespace Naheulbook.Core.Features.Shared;

public class OptionalNaheulbookExecutionContext
{
    public OptionalNaheulbookExecutionContext()
    {
    }

    public OptionalNaheulbookExecutionContext(NaheulbookExecutionContext? executionContext)
    {
        ExecutionExecutionContext = executionContext;
    }

    public NaheulbookExecutionContext? ExecutionExecutionContext { get; set; }
}

public class NaheulbookExecutionContext
{
    public int UserId { get; set; }
}