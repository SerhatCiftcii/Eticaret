namespace Eticaret.WebUI.Models
{

    public class AdminDashboardViewModel
    {
        public int TotalProductCount { get; set; }
        public int ActiveCategoryCount { get; set; }
        public int TotalCategoryCount { get; set; }
        public int LowStockProductCount { get; set; }
        public int TotalCustomerCount { get; set; }
        public int NewOrderCountLast24Hours { get; set; }
        public int PendingOrderCount { get; set; }
        public List<string> CategoryNames { get; set; } // Kategori isimleri için yeni bir özellik
    }

}