namespace MyDashboardApp.Models
{
    public class DashboardViewModel
    {
        public string[] Labels { get; set; } = [];
        public int[] Values { get; set; } = [];
        public List<SalesData> RecentSales { get; set; } = [];

        public int Total => Values.Sum();
        public int Average => Values.Length == 0 ? 0 : Total / Values.Length;
        public string BestMonth => Values.Length == 0 ? "-" : Labels[Array.IndexOf(Values, Values.Max())];
    }
}
