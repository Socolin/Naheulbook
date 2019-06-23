namespace Naheulbook.Core.Utils
{
    public interface IStringCleanupUtil
    {
        string RemoveAccents(string input);
        string RemoveSeparators(string input);
    }

    public class StringCleanupUtil : IStringCleanupUtil
    {
        public string RemoveAccents(string input)
        {
            return StringCleanupHelper.RemoveAccents(input);
        }

        public string RemoveSeparators(string input)
        {
            return input
                .Replace("'", "")
                .Replace("-", "")
                .Replace(" ", "");
        }
    }
}