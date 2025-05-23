using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Group;

public interface IDurationUtil
{
    Task UpdateDurationAsync(int groupId, IList<FighterDurationChanges> fighters);
}

public class DurationUtil(
    IUnitOfWorkFactory unitOfWorkFactory,
    IJsonUtil jsonUtil,
    INotificationSessionFactory notificationSessionFactory
) : IDurationUtil
{
    public async Task UpdateDurationAsync(int groupId, IList<FighterDurationChanges> request)
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var monsterUpdateDurations = request.OfType<MonsterUpdateDuration>().ToList();
            var monsterIds = monsterUpdateDurations.Select(f => f.MonsterId).ToList();
            var monsters = await uow.Monsters.GetWithItemsByGroupAndByIdAsync(groupId, monsterIds);
            if (monsterIds.Count != monsters.Count)
                throw new MonsterNotFoundException(monsters.First(x => !monsterIds.Contains(x.Id)).Id);

            var characterUpdateDurations = request.OfType<CharacterUpdateDuration>().ToList();
            var characterIds = characterUpdateDurations.Select(f => f.CharacterId).ToList();
            var characters = await uow.Characters.GetWithItemsWithModifiersByGroupAndByIdAsync(groupId, characterIds);
            if (characterIds.Count != characters.Count)
                throw new CharacterNotFoundException(characters.First(x => !characterIds.Contains(x.Id)).Id);

            foreach (var (character, changes) in characters.Join(characterUpdateDurations, c => c.Id, c => c.CharacterId, (character, updateDuration) => (character, updateDuration.Changes)))
                UpdateCharacterDuration(character, changes, notificationSession);

            foreach (var (monster, changes) in monsters.Join(monsterUpdateDurations, c => c.Id, c => c.MonsterId, (monster, updateDuration) => (monster, updateDuration.Changes)))
                UpdateMonsterDuration(monster, changes, notificationSession);

            await uow.SaveChangesAsync();
        }

        await notificationSession.CommitAsync();
    }

    private void UpdateMonsterDuration(MonsterEntity monster, IList<IDurationChange> changes, INotificationSession notificationSession)
    {
        var modifiers = jsonUtil.DeserializeOrCreate<List<ActiveStatsModifier>>(monster.Modifiers);

        foreach (var change in changes.OfType<ModifierDurationChange>())
        {
            ApplyChangeOnMonsterModifier(monster, modifiers, change, notificationSession);
        }

        UpdateItemsDuration(monster.Items, changes.OfType<IITemDurationChange>(), notificationSession);

        monster.Modifiers = jsonUtil.Serialize(modifiers);
    }

    private void UpdateItemsDuration(ICollection<ItemEntity> items, IEnumerable<IITemDurationChange> changes, INotificationSession notificationSession)
    {
        foreach (var (item, change) in items.Join(changes.OfType<ItemModifierDurationChange>(), i => i.Id, c => c.ItemId, (item, change) => (item, change)))
        {
            var modifiers = jsonUtil.DeserializeOrCreate<List<ActiveStatsModifier>>(item.Modifiers);
            ApplyChangeOnItemModifier(modifiers, change);
            item.Modifiers = jsonUtil.Serialize(modifiers);
            notificationSession.NotifyItemUpdateModifier(item);
        }
    }

    private void UpdateCharacterDuration(CharacterEntity character, IList<IDurationChange> changes, INotificationSession notificationSession)
    {
        foreach (var change in changes.OfType<ModifierDurationChange>())
        {
            ApplyChangeOnCharacterModifier(character, character.Modifiers, change, notificationSession);
        }

        UpdateItemsDuration(character.Items, changes.OfType<IITemDurationChange>(), notificationSession);
    }

    private void ApplyChangeOnCharacterModifier(CharacterEntity character, ICollection<CharacterModifierEntity> modifiers, ModifierDurationChange change, INotificationSession notificationSession)
    {
        var newModifier = change.Modifier;
        var modifier = modifiers.First(m => m.Id == newModifier.Id);

        if (!newModifier.Active && !modifier.Reusable)
        {
            modifiers.Remove(modifier);
            notificationSession.NotifyCharacterRemoveModifier(character.Id, modifier.Id);
        }
        else
        {
            modifier.IsActive = newModifier.Active;
            modifier.CurrentCombatCount = newModifier.CurrentCombatCount;
            modifier.CurrentLapCount = newModifier.CurrentLapCount;
            modifier.CurrentTimeDuration = newModifier.CurrentTimeDuration;
            notificationSession.NotifyCharacterUpdateModifier(character.Id, modifier);
        }
    }

    private void ApplyChangeOnMonsterModifier(MonsterEntity monster, IList<ActiveStatsModifier> modifiers, IModifierChange change, INotificationSession notificationSession)
    {
        var newModifier = change.Modifier;
        var modifier = modifiers.First(m => m.Id == newModifier.Id);

        if (!newModifier.Active && !modifier.Reusable)
        {
            modifiers.Remove(modifier);
            notificationSession.NotifyMonsterRemoveModifier(monster.Id, modifier.Id);
        }
        else
        {
            modifier.Active = newModifier.Active;
            modifier.CurrentCombatCount = newModifier.CurrentCombatCount;
            modifier.CurrentLapCount = newModifier.CurrentLapCount;
            modifier.CurrentTimeDuration = newModifier.CurrentTimeDuration;
            notificationSession.NotifyMonsterUpdateModifier(monster.Id, modifier);
        }
    }

    private void ApplyChangeOnItemModifier(IList<ActiveStatsModifier> modifiers, IModifierChange change)
    {
        var newModifier = change.Modifier;
        var modifier = modifiers.First(m => m.Id == newModifier.Id);

        if (!newModifier.Active && !modifier.Reusable)
        {
            modifiers.Remove(modifier);
        }
        else
        {
            modifier.Active = newModifier.Active;
            modifier.CurrentCombatCount = newModifier.CurrentCombatCount;
            modifier.CurrentLapCount = newModifier.CurrentLapCount;
            modifier.CurrentTimeDuration = newModifier.CurrentTimeDuration;
        }
    }
}