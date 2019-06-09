namespace Naheulbook.Requests.Requests
{
    public class CreateMonsterTemplateRequest
    {
        public int CategoryId { get; set; }

        public MonsterTemplateRequest Monster { get; set; }
    }
}