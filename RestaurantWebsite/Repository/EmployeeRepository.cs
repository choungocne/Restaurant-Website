using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employees;
        public EmployeeRepository()
        {
            _employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1,
                    FullName = "Nguyễn Văn A",
                    BirthDate =  new DateOnly(2000,02,16),
                    PhoneNumber = "0235168942",
                    Detail = "Có hơn 20 năm kinh nghiệm nấu ăn",
                    Img = "chefs-1.jpg"
                },
            };
        }
        public IEnumerable<Employee> GetAll()
        {
            return _employees;
        }
        public Employee GetById(int Id)
        {
            return _employees.FirstOrDefault(p => p.EmployeeId == Id);
        }
        public void Add(Employee employee)
        {
            employee.EmployeeId = _employees.Max(p => p.EmployeeId) + 1;
            _employees.Add(employee);
        }
        public void Update(Employee employee)
        {
            var index = _employees.FindIndex(p => p.EmployeeId == employee.EmployeeId);
            if (index >= 0)
            {
                _employees[index] = employee;
            }
        }
        public void Delete(int Id)
        {
            var employee = _employees.FirstOrDefault(p => p.EmployeeId == Id);
            if (employee != null)
            {
                _employees.Remove(employee);
            }
        }

        void IEmployeeRepository.Add(Employee employee)
        {
            throw new NotImplementedException();
        }

        void IEmployeeRepository.Update(Employee employee)
        {
            throw new NotImplementedException();
        }

        void IEmployeeRepository.Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
