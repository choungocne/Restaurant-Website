using Microsoft.AspNetCore.Mvc;
using RestaurantWebsite.Repositories;
namespace RestaurantWebsite.ViewComponents
{
    public class DishCategoryViewComponent : ViewComponent
    {
        private readonly DishCategoryRepository _dishCategory;
        public DishCategoryViewComponent(IDishCategoryRepository dishCategoryRepository)
        {
            dishCategoryRepository = _dishCategory;
        }
        public IViewComponentResult Innoke()
        {
            var dishCategory = _dishCategory.GetAll().OrderBy(x=>x.CategoryName);
            return View(dishCategory);
        }
    }
}
