using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repositories
{
    public interface IDishCategoryRepository
    {
        DishCategory Add(DishCategory category);
        DishCategory Update(DishCategory category);
        DishCategory Delete(DishCategory category);
        DishCategory Get(DishCategory category);
        IEnumerable<DishCategory> GetAll();
    }
}
