using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyDashboardApp.Data;
using MyDashboardApp.Hubs;
using MyDashboardApp.Models;

namespace MyDashboardApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDashboardAppContext _context;
        private readonly IHubContext<ChartHub> _chartHub;

        public HomeController(MyDashboardAppContext context, IHubContext<ChartHub> chartHub)
        {
            _context = context;
            _chartHub = chartHub;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Fetch sales data from the database in the order it was added
            var sales = await _context.SalesData.OrderBy(s => s.Id).ToListAsync();

            var model = new DashboardViewModel
            {
                Labels = sales.Select(s => s.Month).ToArray(),
                Values = sales.Select(s => s.Sales).ToArray(),
                RecentSales = sales.TakeLast(5).Reverse().ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Only admins can add data
        public async Task<IActionResult> AddSale([Bind("Month,Sales")] SalesData sale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SalesData.Add(sale);
            await _context.SaveChangesAsync();

            // Push the new point to every open dashboard
            await _chartHub.Clients.All.SendAsync("SaleAdded", sale.Month, sale.Sales);

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
