namespace Naheulbook.Web.Services
{
    public interface IHubGroupUtil
    {
        string GetCharacterGroupName(int characterId);
        string GetGroupGroupName(int groupId);
        string GetLootGroupName(int lootId);
        string GetMonsterGroupName(int monsterId);
    }

    public class HubGroupUtil : IHubGroupUtil
    {
        public string GetCharacterGroupName(int characterId) => $"characters:{characterId}";
        public string GetGroupGroupName(int groupId) => $"groups:{groupId}";
        public string GetLootGroupName(int lootId) => $"loots:{lootId}";
        public string GetMonsterGroupName(int monsterId) => $"monsters:{monsterId}";
    }
}