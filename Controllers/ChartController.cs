using Microsoft.AspNetCore.Mvc;
using MyDashboardApp.Areas.Identity.Data;
using MyDashboardApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace MyDashboardApp.Controllers
{
    public class ChartController : Controller
    {
            private readonly MyDashboardAppContext _context;
            private readonly IHubContext<ChartHub> _chartHub;

        public ChartController(MyDashboardAppContext context, IHubContext<ChartHub> chartHub)
            {
                _context = context;
                _chartHub = chartHub;
        }
            public IActionResult Index()
        {
            // Example: Passing chart data from the controller to the view
            // ViewBag.ChartData = new int[] { 10, 20, 30, 40 };
            //ViewBag.Labels = new string[] { "January", "February", "March", "April" };
            try
            {
                var salesData = _context.SalesData.OrderBy(s => s.Id).ToList();     // Fetch sales data from the database                                                                   
                ViewBag.ChartData = salesData.Select(s => s.Sales).ToArray();      // Prepare data for the chart
                ViewBag.Labels = salesData.Select(s => s.Month).ToArray();
                return View();
            }
            catch (Exception ex)
            {
                // Log the error or show a friendly message
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only admins can add data
        // [Route("Chart/AddData")]
        public async Task<IActionResult> AddData(string month, int sales)
        {
            // Add new data to the database
            var newData = new SalesData { Month = month, Sales = sales };
            _context.SalesData.Add(newData);
            await _context.SaveChangesAsync();

            // Notify clients to update the chart
            await _chartHub.Clients.All.SendAsync("ReceiveChartUpdate");

            return Ok();
        }
    }
}
