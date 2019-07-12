namespace Naheulbook.Web.Responses
{
    public class TakeItemResponse
    {
        public ItemResponse TakenItem { get; set; }
        public int RemainingQuantity { get; set; }
    }
}