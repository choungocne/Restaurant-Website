using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repository
{
    public class EmployeeRepository
    {
        private readonly List<Employee> _employees;
        public EmployeeRepository()
        {
            _employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeID = 1,
                    FullName = "Nguyen Van A",
                    BirthDate =  new DateTime(2000, 2, 16),
                    PhoneNummber = "0235168942"
                },
            };
        }
        public IEnumerable<Employee> GetAll()
        {
            return _employees;
        }
        public Employee GetById(int id)
        {
            return _employees.FirstOrDefault(p => p.EmployeeID == id);
        }
        public void Add(Employee employee)
        {
            employee.EmployeeID = _employees.Max(p => p.EmployeeID) + 1;
            _employees.Add(employee);
        }
        public void Update(Employee employee)
        {
            var index = _employees.FindIndex(p => p.EmployeeID == employee.EmployeeID);
            if (index >= 0)
            {
                _employees[index] = employee;
            }
        }
        public void Delete(int id)
        {
            var employee = _employees.FirstOrDefault(p => p.EmployeeID == id);
            if (employee != null)
            {
                _employees.Remove(employee);
            }
        }
    }
}
