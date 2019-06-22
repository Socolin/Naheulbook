using System;

namespace Naheulbook.Tests.Functional.Code.Utils
{
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
    }
}