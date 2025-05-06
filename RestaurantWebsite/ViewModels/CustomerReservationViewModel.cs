using RestaurantWebsite.Models;

namespace RestaurantWebsite.ViewModels
{
    public class CustomerReservationViewModel
    {
        public TableReservation Reservation { get; set; }
        public OrderService OrderService { get; set; }
        public List<OrderDishViewModel> OrderDishes { get; set; }
        public decimal TotalAmount { get; set; }
        public string StatusDisplay { get; set; }
    }
}
