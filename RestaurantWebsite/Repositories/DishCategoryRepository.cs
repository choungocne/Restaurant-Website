using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repositories
{
    public class DishCategoryRepository : IDishCategoryRepository
    {
        private readonly RestaurantContext _context;
        public DishCategoryRepository(RestaurantContext context)
        {
            _context = context;
        }
        public DishCategory Add(DishCategory category)
        {
            _context.DishCategories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public DishCategory Delete(DishCategory category)
        {
            throw new NotImplementedException();
        }

        public DishCategory Get(string catagoryId)
        {
            return _context.DishCategories.Find(catagoryId);
        }

        public DishCategory Get(DishCategory category)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DishCategory> GetAll()
        {
            return _context.DishCategories;
        }

        public DishCategory Update(DishCategory category)
        {
           _context.Update(category);
            _context.SaveChanges();
            return category;
        }
    }
}
