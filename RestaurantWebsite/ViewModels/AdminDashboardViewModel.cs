using RestaurantWebsite.Models;

namespace RestaurantWebsite.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int CustomerCount { get; set; }
        public int DishCount { get; set; }
        public int ReservationCount { get; set; }
        public int EmployeeCount { get; set; }
        public List<TableReservation> RecentReservations { get; set; } = new List<TableReservation>();

    }
}
