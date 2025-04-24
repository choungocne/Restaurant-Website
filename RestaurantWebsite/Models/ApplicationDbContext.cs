using Microsoft.EntityFrameworkCore;

namespace RestaurantWebsite.Models
{
    public class ApplicationDbContext : DbContext
    {
        public
        ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<DishCategory> DishCategories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DiningTable> DiningTables { get; set; }
        public DbSet<OrderService> OrderServices { get; set; }
        public DbSet<OrderDish> OrderDishes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

    }
}
