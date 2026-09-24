namespace MyDashboardApp.Data
{
    // Resets the sample sales once a day so the public demo stays clean
    public class DemoResetService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DemoResetService> _logger;

        public DemoResetService(IServiceProvider services, ILogger<DemoResetService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

            // Reset on startup too, free hosting plans restart the app after it sleeps
            do
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<MyDashboardAppContext>();
                    await SeedData.ResetSalesAsync(context);
                    _logger.LogInformation("Demo sales data was reset.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Resetting the demo sales data failed.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
