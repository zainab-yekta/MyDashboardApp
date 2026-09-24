using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MyDashboardApp.Hubs
{
    // Clients only listen here, updates are sent from HomeController.AddSale
    [Authorize] // Requires users to be authenticated
    public class ChartHub : Hub
    {
    }
}
