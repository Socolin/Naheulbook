using Microsoft.AspNetCore.SignalR;
using Naheulbook.Core.Services;
using Naheulbook.Web.Hubs;

namespace Naheulbook.Web.Services
{
    public class ChangeNotifier : IChangeNotifier
    {
        private readonly IHubContext<ChangeNotifierHub> _hubContext;

        public ChangeNotifier(IHubContext<ChangeNotifierHub> hubContext)
        {
            _hubContext = hubContext;
        }
    }
}