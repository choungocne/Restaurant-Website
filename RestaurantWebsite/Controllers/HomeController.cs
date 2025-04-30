using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using RestaurantWebsite.ViewModels;

namespace RestaurantWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RestaurantContext _context;

        public HomeController(ILogger<HomeController> logger, RestaurantContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int ? page)
        {
            var employees = _context.Employees.AsNoTracking().OrderBy(p => p.FullName).ToList();
            var dishes = _context.Dishes.AsNoTracking().OrderBy(p => p.DishName).ToList();
            var tables = _context.DiningTables.AsNoTracking().OrderBy(p => p.TableName).ToList();
            var viewModel = new EmployeeDishViewModel
            {
                Employees = employees,
                Dishes = dishes,
                DiningTables = tables
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