namespace Naheulbook.Core.Models
{
    public interface IOptionalNaheulbookExecutionContext
    {
        int UserId { get; set; }
    }

    public class NaheulbookExecutionContext : IOptionalNaheulbookExecutionContext
    {
        public int UserId { get; set; }
    }
}