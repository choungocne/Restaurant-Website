using RestaurantWebsite.Models;

namespace RestaurantWebsite.Services
{

    public interface IDishService
    {
        Task<IEnumerable<Dish>> GetAllAsync();
        Task<Dish> GetByIdAsync(int id);
        Task AddAsync(Dish dish);
        Task UpdateAsync(Dish dish);
        Task DeleteAsync(int id);
    }
}

