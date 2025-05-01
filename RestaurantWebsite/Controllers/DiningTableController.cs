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

        // GET: DiningTable/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diningTable = await _context.DiningTables
                .Include(t => t.OrderServices)
                .FirstOrDefaultAsync(m => m.TableId == id);

            if (diningTable == null)
            {
                return NotFound();
            }

            return View(diningTable);
        }

        // GET: DiningTable/Available
        public async Task<IActionResult> Available()
        {
            // Get tables that have IsAvailable = true
            var availableTables = await _context.DiningTables
                .Where(t => t.IsAvailable)
                .ToListAsync();

            return View(availableTables);
        }

        // GET: DiningTable/Filter
        public async Task<IActionResult> Filter(string location, bool? isAvailable)
        {
            var query = _context.DiningTables.AsQueryable();

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(t => t.Location == location);
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(t => t.IsAvailable == isAvailable.Value);
            }

            var tables = await query.ToListAsync();

            // Get all unique locations for the filter dropdown
            ViewBag.Locations = await _context.DiningTables
                .Select(t => t.Location)
                .Distinct()
                .ToListAsync();

            // Save filter values for the view
            ViewBag.SelectedLocation = location;
            ViewBag.IsAvailable = isAvailable;

            return View("Index", tables);
        }
    }
}