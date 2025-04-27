using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using System.Collections.Generic;

namespace RestaurantWebsite.Repositories
{
    public class DishRepository: IDishRepository
    {
        private readonly RestaurantContext _context;

        public DishRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dish>> GetAllAsync()
        {
            var dishes = await _context.Dishes
         .Include(d => d.Category)
         .ToListAsync();
            Console.WriteLine($"Retrieved {dishes.Count} dishes");
            return dishes;
        }

        public async Task<Dish> GetByIdAsync(int id)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.DishId == id);
        }

        public async Task AddAsync(Dish dish)
        {
            await _context.Dishes.AddAsync(dish);
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"Saved {result} changes for new dish: {dish.DishName}");
        }

        public async Task UpdateAsync(Dish dish)
        {
            _context.Dishes.Update(dish);
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"Saved {result} changes for updated dish: {dish.DishName}");
        }

        public async Task DeleteAsync(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }
        }
    }
    }