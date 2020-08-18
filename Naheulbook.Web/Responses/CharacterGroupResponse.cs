using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses
{
    public class CharacterGroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public GroupConfig Config { get; set; } = null!;
    }
}