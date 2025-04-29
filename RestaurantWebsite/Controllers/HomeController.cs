using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using RestaurantWebsite.Repositories;
using RestaurantWebsite.Repository;
using X.PagedList;

namespace RestaurantWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDishRepository _dishRepository;
        RestaurantContext db = new RestaurantContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var employees = db.Employees.AsNoTracking().OrderBy(p => p.FullName).ToList();
            var dishes = db.Dishes.AsNoTracking().OrderBy(p => p.DishName).ToList();

            var viewModel = new EmployeeDishViewModel
            {
                Employees = employees,
                Dishes = dishes
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
