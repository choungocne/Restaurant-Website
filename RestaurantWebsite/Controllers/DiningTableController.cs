using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantWebsite.Controllers
{
    public class DiningTableController : Controller
    {
        private readonly RestaurantContext _context;

        public DiningTableController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: DiningTable
        public async Task<IActionResult> Index()
        {
            var tables = await _context.DiningTables.ToListAsync();
            return View(tables);
        }

       
    }
}