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

        public IActionResult Index(int ? page)
        {
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstnhanvien = db.Employees.AsNoTracking().OrderBy(p => p.FullName);
            var lstsanpham = db.Dishes.AsNoTracking().OrderBy(p => p.DishName);
            PagedList<Employee> lst = new PagedList<Employee>(lstnhanvien, pageNumber, pageSize);
            PagedList<Dish> l = new PagedList<Dish>(lstsanpham, pageNumber, pageSize);
            return View(lst);
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
