using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests
{
    public class CreateItemRequest
    {
        public int ItemTemplateId { get; set; }
        public ItemData ItemData { get; set; } = null!;
    }
}