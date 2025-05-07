using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using RestaurantWebsite.Services;
using System.Text.Json;

namespace RestaurantWebsite.Controllers
{
    public class DishesController : Controller
    {
        private readonly IDishService _dishService;
        private readonly RestaurantContext _context;

        public DishesController(IDishService dishService, RestaurantContext context)
        {
            _dishService = dishService;
            _context = context;
        }

        // GET: Dishes
        public async Task<IActionResult> Index()
        {
            var dishes = await _dishService.GetAllAsync();
            return View(dishes);
        }

      

       
    }
}