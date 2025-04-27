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

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _dishService.GetByIdAsync(id.Value);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dishes/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.DishCategories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Dishes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishName,UnitPrice,CategoryId,Img")] Dish dish)
        {
            if (ModelState.IsValid)
            {
                // Lưu dữ liệu mới
                await _dishService.AddAsync(dish);
                // Lưu thông báo thành công vào TempData
                TempData["SuccessMessage"] = $"Created dish: {dish.DishName}";
                // Trả về danh sách món ăn
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.DishCategories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _dishService.GetByIdAsync(id.Value);
            if (dish == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.DishCategories, "CategoryId", "CategoryName", dish.CategoryId);
            // Lưu dữ liệu ban đầu vào ViewBag để hiển thị nếu cần
            ViewBag.OriginalDish = JsonSerializer.Serialize(dish);
            return View(dish);
        }

        // POST: Dishes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DishId,DishName,UnitPrice,CategoryId,Img")] Dish dish)
        {
            if (id != dish.DishId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy dữ liệu ban đầu
                    var originalDish = await _dishService.GetByIdAsync(id);
                    if (originalDish == null)
                    {
                        return NotFound();
                    }
                    // Lưu dữ liệu ban đầu vào TempData
                    TempData["OriginalDish"] = JsonSerializer.Serialize(originalDish);
                    TempData["SuccessMessage"] = $"Updated dish: {dish.DishName}";
                    // Cập nhật dữ liệu
                    await _dishService.UpdateAsync(dish);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _dishService.GetByIdAsync(dish.DishId) == null)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.DishCategories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _dishService.GetByIdAsync(id.Value);
            if (dish == null)
            {
                return NotFound();
            }

            // Lưu dữ liệu ban đầu vào ViewBag
            ViewBag.OriginalDish = JsonSerializer.Serialize(dish);
            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Lấy dữ liệu ban đầu
            var originalDish = await _dishService.GetByIdAsync(id);
            if (originalDish == null)
            {
                return NotFound();
            }
            // Lưu dữ liệu ban đầu và thông báo vào TempData
            TempData["OriginalDish"] = JsonSerializer.Serialize(originalDish);
            TempData["SuccessMessage"] = $"Deleted dish: {originalDish.DishName}";
            // Xóa món ăn
            await _dishService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}