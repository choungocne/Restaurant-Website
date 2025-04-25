using RestaurantWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace RestaurantWebsite.Repository
{
    public class DishRepository : IDishRepository
    {
        private readonly ApplicationDbContext _context;

        public DishRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Dish> GetAll()
        {
            return _context.Dishes.Include(d => d.Category).ToList();
        }

        public Dish GetById(int id)
        {
            return _context.Dishes.Include(d => d.Category).FirstOrDefault(d => d.DishId == id);
        }

        public void Add(Dish dish)
        {
            _context.Dishes.Add(dish);
            _context.SaveChanges();
        }

        public void Update(Dish dish)
        {
            _context.Dishes.Update(dish);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var dish = GetById(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                _context.SaveChanges();
            }
        }
    }
}
