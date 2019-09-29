using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface IDurationUtil
    {
        Task UpdateDurationAsync(int groupId, IList<FighterDurationChanges> fighters);
    }

    public class DurationUtil : IDurationUtil
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IJsonUtil _jsonUtil;
        private readonly INotificationSessionFactory _notificationSessionFactory;

        public DurationUtil(
            IUnitOfWorkFactory unitOfWorkFactory,
            IJsonUtil jsonUtil,
            INotificationSessionFactory notificationSessionFactory
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _jsonUtil = jsonUtil;
            _notificationSessionFactory = notificationSessionFactory;
        }

        public async Task UpdateDurationAsync(int groupId, IList<FighterDurationChanges> request)
        {
            var notificationSession = _notificationSessionFactory.CreateSession();

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
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

        private void UpdateMonsterDuration(Monster monster, IList<IDurationChange> changes, INotificationSession notificationSession)
        {
            var modifiers = _jsonUtil.DeserializeOrCreate<List<ActiveStatsModifier>>(monster.Modifiers);

            foreach (var change in changes.OfType<ModifierDurationChange>())
            {
                ApplyChangeOnMonsterModifier(monster, modifiers, change, notificationSession);
            }

            UpdateItemsDuration(monster.Items, changes.OfType<IITemDurationChange>(), notificationSession);

            monster.Modifiers = _jsonUtil.Serialize(modifiers);
        }

        private void UpdateItemsDuration(ICollection<Item> items, IEnumerable<IITemDurationChange> changes, INotificationSession notificationSession)
        {
            foreach (var (item, change) in items.Join(changes.OfType<ItemModifierDurationChange>(), i => i.Id, c => c.ItemId, (item, change) => (item, change)))
            {
                var modifiers = _jsonUtil.DeserializeOrCreate<List<ActiveStatsModifier>>(item.Modifiers);
                ApplyChangeOnItemModifier(item, modifiers, change, notificationSession);
                item.Modifiers = _jsonUtil.Serialize(modifiers);
            }
        }

        private void UpdateCharacterDuration(Character character, IList<IDurationChange> changes, INotificationSession notificationSession)
        {
            foreach (var change in changes.OfType<ModifierDurationChange>())
            {
                ApplyChangeOnCharacterModifier(character, character.Modifiers, change, notificationSession);
            }

            UpdateItemsDuration(character.Items, changes.OfType<IITemDurationChange>(), notificationSession);
        }

        private void ApplyChangeOnCharacterModifier(Character character, ICollection<CharacterModifier> modifiers, ModifierDurationChange change, INotificationSession notificationSession)
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

        private void ApplyChangeOnMonsterModifier(Monster monster, IList<ActiveStatsModifier> modifiers, IModifierChange change, INotificationSession notificationSession)
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

        private void ApplyChangeOnItemModifier(Item item, IList<ActiveStatsModifier> modifiers, IModifierChange change, INotificationSession notificationSession)
        {
            var newModifier = change.Modifier;
            var modifier = modifiers.First(m => m.Id == newModifier.Id);

            if (!newModifier.Active && !modifier.Reusable)
            {
                modifiers.Remove(modifier);
                notificationSession.NotifyItemUpdateModifier(item);
            }
            else
            {
                modifier.Active = newModifier.Active;
                modifier.CurrentCombatCount = newModifier.CurrentCombatCount;
                modifier.CurrentLapCount = newModifier.CurrentLapCount;
                modifier.CurrentTimeDuration = newModifier.CurrentTimeDuration;
                notificationSession.NotifyItemUpdateModifier(item);
            }
        }
    }
}