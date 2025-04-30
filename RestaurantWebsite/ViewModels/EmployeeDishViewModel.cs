using RestaurantWebsite.Models;
using X.PagedList;

namespace RestaurantWebsite.ViewModels
{
    public class EmployeeDishViewModel
    {
        public List<Employee> Employees { get; set; }
        public List<Dish> Dishes { get; set; }
        public IEnumerable<DiningTable> DiningTables { get; set; } = new List<DiningTable>();
        public IEnumerable<OrderService> OrderServices { get; set; } = new List<OrderService>();
    }
}
