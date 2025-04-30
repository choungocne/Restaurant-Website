using Microsoft.AspNetCore.Mvc;
using RestaurantWebsite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantWebsite.Controllers
{
    [Authorize] // Yêu cầu đăng nhập
    public class CustomerController : Controller
    {
        private readonly RestaurantContext _context;

        public CustomerController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: Hiển thị hồ sơ khách hàng
        public async Task<IActionResult> Index()
        {
            // Lấy UserId từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserAccounts.UserId == userId);

            if (customer == null)
            {
                // Nếu chưa có hồ sơ, tạo mới
                customer = new Customer { UserId = userId };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            return View(customer);
        }

        // GET: Form chỉnh sửa hồ sơ
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Cập nhật hồ sơ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin
            existingCustomer.CustomerName = customer.CustomerName;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.Address = customer.Address;

            try
            {
                _context.Update(existingCustomer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hồ sơ đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Không thể cập nhật hồ sơ. Vui lòng thử lại.");
                return View(customer);
            }
        }

        // GET: Form xác nhận xóa tài khoản
        public async Task<IActionResult> Delete()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Xóa tài khoản
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tài khoản đã được xóa thành công!";
                // Đăng xuất sau khi xóa
                return RedirectToAction("Logout", "Account");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}