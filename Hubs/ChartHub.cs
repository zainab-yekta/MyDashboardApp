using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


namespace MyDashboardApp.Hubs
{
    [Authorize] // Requires users to be authenticated
    public class ChartHub : Hub
    {
        public async Task UpdateChart()
        {
            // Notify all connected clients to update the chart
            await Clients.All.SendAsync("ReceiveChartUpdate");
        }
    }
}
