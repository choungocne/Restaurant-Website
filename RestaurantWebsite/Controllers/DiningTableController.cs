using Microsoft.AspNetCore.Mvc;
using RestaurantWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using X.PagedList;

namespace RestaurantWebsite.Controllers
{
    public class DiningTableController : Controller
    {
        private readonly RestaurantContext _context;

        public DiningTableController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: Hiển thị danh sách bàn với phân trang
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10; // Số bàn mỗi trang
            int pageNumber = (page ?? 1); // Trang mặc định là 1
            var tables = await _context.DiningTables
                .AsNoTracking()
                .OrderBy(t => t.TableName)
                .ToListAsync();
            return View(tables);
        }
    }
}