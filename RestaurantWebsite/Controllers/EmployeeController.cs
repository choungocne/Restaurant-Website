using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using RestaurantWebsite.Models;
using RestaurantWebsite.Repository;

namespace RestaurantWebsite.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(IEmployeeRepository employeeRepository, IWebHostEnvironment webHostEnvironment)
        {
            _employeeRepository = employeeRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            try
            {
                // Get all employees
                var employees = _employeeRepository.GetAll();

                // Add debug information
                ViewBag.EmployeeCount = employees.Count();
                ViewBag.HasData = employees.Any();

                return View(employees);
            }
            catch (Exception ex)
            {
                // Handle and display any exceptions
                ViewBag.ErrorMessage = $"Error retrieving employee data: {ex.Message}";
                ViewBag.StackTrace = ex.StackTrace;
                return View(new List<Employee>());
            }
        }

      

    }
}