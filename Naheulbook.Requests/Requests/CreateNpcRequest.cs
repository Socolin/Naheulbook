using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests
{
    public class CreateNpcRequest
    {
        public string Name { get; set; }
        public NpcData Data { get; set; }
    }
}