using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repositories
{
    public interface IDiningTableRepository
    {
       
        IEnumerable<DiningTable> GetAll ();
        DiningTable GetByName (string name);
    }
}
