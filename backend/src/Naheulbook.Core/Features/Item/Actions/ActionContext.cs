using Naheulbook.Data.Extensions.UnitOfWorks;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Features.Item.Actions;

public class ActionContext(ItemEntity usedItem, CharacterEntity sourceCharacter, CharacterEntity targetCharacter, IUnitOfWork unitOfWork)
{
    public ItemEntity UsedItem { get; } = usedItem;
    public CharacterEntity SourceCharacter { get; } = sourceCharacter;
    public CharacterEntity TargetCharacter { get; } = targetCharacter;
    public IUnitOfWork UnitOfWork { get; } = unitOfWork;
}