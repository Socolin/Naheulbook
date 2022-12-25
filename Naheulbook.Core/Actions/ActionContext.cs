using Naheulbook.Data.Models;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Actions
{
    public class ActionContext
    {
        public ItemEntity UsedItem { get; }
        public CharacterEntity SourceCharacter { get; }
        public CharacterEntity TargetCharacter { get; }
        public IUnitOfWork UnitOfWork { get; }

        public ActionContext(ItemEntity usedItem, CharacterEntity sourceCharacter, CharacterEntity targetCharacter, IUnitOfWork unitOfWork)
        {
            UsedItem = usedItem;
            SourceCharacter = sourceCharacter;
            TargetCharacter = targetCharacter;
            UnitOfWork = unitOfWork;
        }
    }
}