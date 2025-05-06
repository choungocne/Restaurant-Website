namespace RestaurantWebsite.ViewModels
{
    public class CustomerReservationsListViewModel
    {
        public List<CustomerReservationViewModel> Reservations { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
