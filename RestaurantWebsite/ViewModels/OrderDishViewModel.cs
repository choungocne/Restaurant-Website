namespace RestaurantWebsite.ViewModels
{
    public class OrderDishViewModel
    {
        public string DishName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public string Note { get; set; }
    }
}
