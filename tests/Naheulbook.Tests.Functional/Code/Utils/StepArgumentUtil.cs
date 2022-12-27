using System;

namespace Naheulbook.Tests.Functional.Code.Utils;

public static class StepArgumentUtil
{
    public static int ParseQuantity(string quantity)
    {
        if (quantity == "an" || quantity == "a")
            return 1;

        return int.Parse(quantity);
    }

    public static int ParseIndex(string indexString)
    {
        if (indexString == "first" || indexString == "1st")
            return 0;

        if (indexString == "second" || indexString == "2nd")
            return 1;

        if (indexString == "third" || indexString == "3rd")
            return 2;

        if (indexString == "fourth" || indexString == "4th")
            return 3;

        throw new NotSupportedException(indexString + " is not a recognized string");
    }


    public static int ParseNth(string nthString)
    {
        if (nthString == "first" || nthString == "1st")
            return 1;

        if (nthString == "second" || nthString == "2nd")
            return 2;

        if (nthString == "third" || nthString == "3rd")
            return 3;

        if (nthString.EndsWith("th"))
        {
            var numberString = nthString.Substring(0, nthString.LastIndexOf("th", StringComparison.Ordinal));
            if (int.TryParse(numberString, out var number))
                return number;
        }

        throw new NotSupportedException(nthString + " is not a recognized string");
    }
}