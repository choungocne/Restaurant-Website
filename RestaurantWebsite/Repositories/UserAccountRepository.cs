using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;

namespace RestaurantWebsite.Repository
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly RestaurantContext _context;

        public UserAccountRepository(RestaurantContext context)
        {
            _context = context;
        }

        public IEnumerable<UserAccount> GetAll()
        {
            return _context.UserAccounts
                .Include(u => u.Employee)
                .Include(u => u.Customer)
                .ToList();
        }

        public UserAccount GetById(int id)
        {
            return _context.UserAccounts
                .Include(u => u.Employee)
                .Include(u => u.Customer)
                .FirstOrDefault(u => u.UserId == id);
        }

        public UserAccount GetByUsername(string username)
        {
            return _context.UserAccounts
                .FirstOrDefault(u => u.Username == username);
        }

        public void Add(UserAccount user)
        {
            user.CreatedAt = DateTime.Now;
            _context.UserAccounts.Add(user);
            _context.SaveChanges();
        }

        public void Update(UserAccount user)
        {
            _context.UserAccounts.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _context.UserAccounts.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
