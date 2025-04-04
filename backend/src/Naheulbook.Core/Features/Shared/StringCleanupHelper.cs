using System.Globalization;
using System.Linq;
using System.Text;

namespace Naheulbook.Core.Features.Shared;

public class StringCleanupHelper
{
    public static string RemoveAccents(string input)
    {
        return new string(input
            .Normalize(NormalizationForm.FormD)
            .ToCharArray()
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
    }
}