using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantWebsite.Models;
using RestaurantWebsite.Repository;

namespace RestaurantWebsite.Controllers
{
    public class DishController : Controller
    {
        private readonly IDishRepository _repo;
        private readonly ApplicationDbContext _context;

        public DishController(IDishRepository repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_repo.GetAll());
        }

        public IActionResult Add()
        {
            ViewBag.Categories = new SelectList(_context.DishCategories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public IActionResult Add(Dish dish)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(dish);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(_context.DishCategories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        public IActionResult Update(int id)
        {
            var dish = _repo.GetById(id);
            if (dish == null) return NotFound();
            ViewBag.Categories = new SelectList(_context.DishCategories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        [HttpPost]
        public IActionResult Update(Dish dish)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(dish);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(_context.DishCategories, "CategoryId", "CategoryName", dish.CategoryId);
            return View(dish);
        }

        public IActionResult Delete(int id)
        {
            var dish = _repo.GetById(id);
            return View(dish);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }

        public IActionResult Display(int id)
        {
            var dish = _repo.GetById(id);
            return View(dish);
        }
    }
}
