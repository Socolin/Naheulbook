namespace Naheulbook.Core.Utils
{
    public interface IStringCleanupUtil
    {
        string RemoveAccents(string input);
    }

    public class StringCleanupUtil : IStringCleanupUtil
    {
        public string RemoveAccents(string input)
        {
            return StringCleanupHelper.RemoveAccents(input);
        }
    }
}