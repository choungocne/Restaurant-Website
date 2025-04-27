using RestaurantWebsite.Models;
using RestaurantWebsite.Repositories;
using System.Collections.Generic;

namespace RestaurantWebsite.Services
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _dishRepository;

        public DishService(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public async Task<IEnumerable<Dish>> GetAllAsync()
        {
            return await _dishRepository.GetAllAsync();
        }

        public async Task<Dish> GetByIdAsync(int id)
        {
            return await _dishRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Dish dish)
        {
            dish.CreatedAt = DateTime.UtcNow;
            await _dishRepository.AddAsync(dish);
        }

        public async Task UpdateAsync(Dish dish)
        {
            await _dishRepository.UpdateAsync(dish);
        }

        public async Task DeleteAsync(int id)
        {
            await _dishRepository.DeleteAsync(id);
        }
    }
}