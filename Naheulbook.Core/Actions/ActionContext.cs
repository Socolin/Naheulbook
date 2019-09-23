using Naheulbook.Data.Models;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Actions
{
    public class ActionContext
    {
        public Item UsedItem { get; }
        public Character SourceCharacter { get; }
        public Character TargetCharacter { get; }
        public IUnitOfWork UnitOfWork { get; }

        public ActionContext(Item usedItem, Character sourceCharacter, Character targetCharacter, IUnitOfWork unitOfWork)
        {
            UsedItem = usedItem;
            SourceCharacter = sourceCharacter;
            TargetCharacter = targetCharacter;
            UnitOfWork = unitOfWork;
        }
    }
}