using System.ComponentModel.DataAnnotations;

namespace MyDashboardApp.Models
{
    public class SalesData
    {
        public int Id { get; set; } // Primary key

        [Required]
        [StringLength(20)]
        public string Month { get; set; } = string.Empty;

        [Range(0, 1_000_000)]
        public int Sales { get; set; }
    }
}
