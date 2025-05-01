using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repositories
{
    public class CustomerRepository
    {
        private static List<Customer> Customers = new List<Customer>();
        public static IEnumerable<Customer> AllCustomers
        {
            get { return Customers; }
        }
    }
}
