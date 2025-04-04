using System.Threading.Tasks;
using Naheulbook.Core.Notifications;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Item.Actions;

public interface IActionExecutor
{
    Task ExecuteAsync(NhbkAction action, ActionContext context, INotificationSession notificationSession);
}