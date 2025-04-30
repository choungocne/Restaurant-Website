using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repositories
{
    public class DinningTableRepository : IDiningTableRepository
    {
        private readonly RestaurantContext _context;
        public  DinningTableRepository (RestaurantContext context)
        {
            _context = context;
        }

       

        public IEnumerable<DiningTable> GetAll()
        { 
            return _context.ToList();
        }

        

        public DiningTable GetByName(string name)
        {
            return _context.DiningTables.Find(name);
        }
    }
}
