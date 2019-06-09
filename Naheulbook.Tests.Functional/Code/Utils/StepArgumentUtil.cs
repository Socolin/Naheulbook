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
    }
}