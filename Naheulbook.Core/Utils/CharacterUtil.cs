using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface ICharacterUtil
    {
        void ApplyCharactersChange(NaheulbookExecutionContext executionContext, PatchCharacterRequest request, Character character, List<Task> notificationTasks);
    }

    public class CharacterUtil : ICharacterUtil
    {
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IChangeNotifier _changeNotifier;
        private readonly IJsonUtil _jsonUtil;
        private readonly ICharacterHistoryUtil _characterHistoryUtil;

        public CharacterUtil(
            IAuthorizationUtil authorizationUtil,
            IChangeNotifier changeNotifier,
            ICharacterHistoryUtil characterHistoryUtil,
            IJsonUtil jsonUtil
        )
        {
            _authorizationUtil = authorizationUtil;
            _changeNotifier = changeNotifier;
            _characterHistoryUtil = characterHistoryUtil;
            _jsonUtil = jsonUtil;
        }

        public void ApplyCharactersChange(NaheulbookExecutionContext executionContext, PatchCharacterRequest request, Character character, List<Task> notificationTasks)
        {
            if (request.Debilibeuk.HasValue)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                var gmData = _jsonUtil.Deserialize<CharacterGmData>(character.GmData) ?? new CharacterGmData();
                gmData.Debilibeuk = request.Debilibeuk.Value;
                character.GmData = _jsonUtil.Serialize(gmData);
            }

            if (request.Mankdebol.HasValue)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                var gmData = _jsonUtil.Deserialize<CharacterGmData>(character.GmData) ?? new CharacterGmData();;
                gmData.Mankdebol = request.Mankdebol.Value;
                character.GmData = _jsonUtil.Serialize(gmData);
            }

            if (request.IsActive.HasValue)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                character.IsActive = request.IsActive.Value;
            }

            if (request.Color != null)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                character.Color = request.Color;
            }

            if (request.OwnerId != null)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                character.OwnerId = request.OwnerId.Value;
            }

            if (request.Target != null)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                if (request.Target.IsMonster)
                {
                    character.TargetedCharacterId = null;
                    character.TargetedMonsterId = request.Target.Id;
                }
                else
                {
                    character.TargetedMonsterId = null;
                    character.TargetedCharacterId = request.Target.Id;
                }
            }

            if (request.Ev.HasValue)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeEv(character, character.Ev, request.Ev));
                character.Ev = request.Ev;
            }

            if (request.Ea.HasValue)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeEa(character, character.Ea, request.Ea));
                character.Ea = request.Ea;
            }

            if (request.FatePoint.HasValue)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeFatePoint(character, character.FatePoint, request.FatePoint));
                character.FatePoint = request.FatePoint.Value;
            }

            if (request.Experience.HasValue)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeExperience(character, character.Experience, request.Experience));
                character.Experience = request.Experience.Value;
            }

            if (request.Sex != null)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeSex(character, character.Sex, request.Sex));
                character.Sex = request.Sex;
            }

            if (request.Name != null)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeName(character, character.Name, request.Name));
                character.Name = request.Name;
            }

            NotifyCharacterChanges(request, character, notificationTasks);
        }

        private void NotifyCharacterChanges(PatchCharacterRequest request, Character character, ICollection<Task> notificationTasks)
        {
            if (request.Debilibeuk.HasValue || request.Mankdebol.HasValue)
            {
                var gmData = _jsonUtil.Deserialize<CharacterGmData>(character.GmData);
                notificationTasks.Add(_changeNotifier.NotifyCharacterGmChangeDataAsync(character, gmData));
            }

            if (request.IsActive.HasValue)
                notificationTasks.Add(_changeNotifier.NotifyCharacterGmChangeActive(character));

            if (request.Color != null)
                notificationTasks.Add(_changeNotifier.NotifyCharacterGmChangeColorAsync(character));

            if (request.OwnerId != null)
                character.OwnerId = request.OwnerId.Value;

            if (request.Target != null)
                notificationTasks.Add(_changeNotifier.NotifyCharacterGmChangeTarget(character, request.Target));

            if (request.Ev.HasValue)
                notificationTasks.Add(_changeNotifier.NotifyCharacterChangeEvAsync(character));

            if (request.Ea.HasValue)
                notificationTasks.Add(_changeNotifier.NotifyCharacterChangeEaAsync(character));

            if (request.FatePoint.HasValue)
                notificationTasks.Add(_changeNotifier.NotifyCharacterChangeFatePointAsync(character));

            if (request.Experience.HasValue)
                notificationTasks.Add(_changeNotifier.NotifyCharacterChangeExperienceAsync(character));

            if (request.Sex != null)
                notificationTasks.Add(_changeNotifier.NotifyCharacterChangeSexAsync(character));

            if (request.Name != null)
                notificationTasks.Add(_changeNotifier.NotifyCharacterChangeNameAsync(character));
        }
    }
}