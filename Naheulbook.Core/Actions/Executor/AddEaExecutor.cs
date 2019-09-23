using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Actions.Executor
{
    public interface IAddEaExecutor : IActionExecutor
    {
    }

    public class AddEaExecutor : IAddEaExecutor
    {
        private const string ActionType = "addEa";
        private readonly ICharacterHistoryUtil _characterHistoryUtil;

        public AddEaExecutor(ICharacterHistoryUtil characterHistoryUtil)
        {
            _characterHistoryUtil = characterHistoryUtil;
        }

        public Task ExecuteAsync(
            NhbkAction action,
            ActionContext context,
            INotificationSession notificationSession
        )
        {
            if (action.Type != ActionType)
                throw new InvalidActionTypeException(action.Type, ActionType);
            if (!action.Data.Ea.HasValue)
                throw new InvalidActionDataException(action.Type);
            if (!context.TargetCharacter.Ea.HasValue) // DO not touch ea of character that does no have any
                return Task.CompletedTask;

            var oldEa = context.TargetCharacter.Ea;
            var newEa = oldEa + action.Data.Ea.Value;

            context.TargetCharacter.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeEa(context.TargetCharacter, oldEa, newEa));
            context.TargetCharacter.Ea = newEa;
            notificationSession.NotifyCharacterChangeEa(context.TargetCharacter);

            return Task.CompletedTask;
        }
    }
}