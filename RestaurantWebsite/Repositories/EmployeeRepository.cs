using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly RestaurantContext _context;

        public EmployeeRepository(RestaurantContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAll()
        {
            try
            {
                return _context.Employees.ToList();
            }
            catch (Exception ex)
            {
                // You could log the exception here
                Console.WriteLine($"Error retrieving employees: {ex.Message}");
                return new List<Employee>();
            }
        }

        public Employee GetById(int id)
        {
            return _context.Employees.Find(id);
        }

        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }
    }
}