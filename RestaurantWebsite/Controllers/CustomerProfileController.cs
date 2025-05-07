using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RestaurantWebsite.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.ViewModels;

namespace RestaurantWebsite.Controllers
{
    [Authorize] // Ensures only logged-in users can access this controller
    public class CustomerProfileController : Controller
    {
        private readonly RestaurantContext _context;

        public CustomerProfileController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: CustomerProfile
        public async Task<IActionResult> Index()
        {
            // Get the current user's username
            var username = User.Identity.Name;

            // Find the user account
            var userAccount = _context.UserAccounts.FirstOrDefault(u => u.Username == username);
            if (userAccount == null)
            {
                return NotFound();
            }

            // Find the customer associated with this user account
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == userAccount.CustomerId);
            if (customer == null)
            {
                return NotFound();
            }

            // Pass the customer data to the view
            return View(customer);
        }

        // GET: CustomerProfile/Edit
        public async Task<IActionResult> Edit()
        {
            var username = User.Identity.Name;

            var userAccount = _context.UserAccounts.FirstOrDefault(u => u.Username == username);
            if (userAccount == null)
            {
                return NotFound();
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == userAccount.CustomerId);
            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.UserAccount = userAccount;
            return View(customer);
        }

        // POST: CustomerProfile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer, string email, IFormFile imageFile)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update customer information
                    var existingCustomer = _context.Customers.Find(id);
                    if (existingCustomer == null)
                    {
                        return NotFound();
                    }

                    existingCustomer.CustomerName = customer.CustomerName;
                    existingCustomer.Address = customer.Address;
                    existingCustomer.PhoneNumber = customer.PhoneNumber;
                    existingCustomer.DateOfBirth = customer.DateOfBirth;

                    // Process image upload if provided
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Create a unique filename
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/customers", fileName);

                        // Ensure directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        // Update the image path in the database
                        existingCustomer.Img = "/images/customers/" + fileName;
                    }

                    // Update user account email if provided
                    if (!string.IsNullOrEmpty(email))
                    {
                        var userAccount = _context.UserAccounts.FirstOrDefault(u => u.CustomerId == id);
                        if (userAccount != null)
                        {
                            userAccount.Email = email;
                            _context.Update(userAccount);
                        }
                    }

                    _context.Update(existingCustomer);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);

                   
                    ViewBag.UserAccount = _context.UserAccounts.FirstOrDefault(u => u.CustomerId == id);
                    return View(customer);
                }
            }

            ViewBag.UserAccount = _context.UserAccounts.FirstOrDefault(u => u.CustomerId == id);
            return View(customer);
        }

        // GET: CustomerProfile/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: CustomerProfile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ModelState.AddModelError(string.Empty, "All fields are required.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "New password and confirmation do not match.");
                return View();
            }

            // Get current user
            var username = User.Identity.Name;
            var userAccount = _context.UserAccounts.FirstOrDefault(u => u.Username == username);

            if (userAccount == null)
            {
                return NotFound();
            }

            // Validate current password
            // In a real application, you would use a proper password hashing mechanism
            if (userAccount.PasswordHash != currentPassword) // This is simplified; use proper hashing in production
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                return View();
            }

            // Update password
            userAccount.PasswordHash = newPassword; // In production, hash the password
            _context.Update(userAccount);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MyReservations()
        {
            var username = User.Identity.Name;
            var userAccount = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Username == username);
            if (userAccount?.CustomerId == null)
                return Unauthorized();

            int customerId = userAccount.CustomerId.Value;

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return NotFound();

            var reservations = await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.OrderService).ThenInclude(os => os.OrderDishes).ThenInclude(od => od.Dish)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var viewModel = new CustomerReservationsListViewModel
            {
                CustomerId = customerId,
                CustomerName = customer.CustomerName,
                Reservations = new List<CustomerReservationViewModel>()
            };

            foreach (var reservation in reservations)
            {
                var orderService = reservation.OrderService;
                decimal totalAmount = 0;
                var orderDishViewModels = new List<OrderDishViewModel>();

                if (orderService?.OrderDishes != null)
                {
                    foreach (var orderDish in orderService.OrderDishes)
                    {
                        decimal dishTotal = (orderDish.UnitPrice ?? 0) * (orderDish.Quantity ?? 0);
                        totalAmount += dishTotal;

                        orderDishViewModels.Add(new OrderDishViewModel
                        {
                            DishName = orderDish.Dish?.DishName ?? "Không xác định",
                            Quantity = orderDish.Quantity ?? 0,
                            UnitPrice = orderDish.UnitPrice ?? 0,
                            SubTotal = dishTotal,
                            Note = orderDish.Note
                        });
                    }
                }

                string statusDisplay = GetStatusDisplay(reservation, orderService);

                viewModel.Reservations.Add(new CustomerReservationViewModel
                {
                    Reservation = reservation,
                    OrderService = orderService,
                    OrderDishes = orderDishViewModels,
                    TotalAmount = totalAmount,
                    StatusDisplay = statusDisplay
                });
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ReservationDetail(int reservationId)
        {
            var reservation = await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Customer)
                .Include(r => r.OrderService).ThenInclude(os => os.OrderDishes).ThenInclude(od => od.Dish)
                .Include(r => r.OrderService).ThenInclude(os => os.Payments)
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null) return NotFound();

            decimal totalAmount = 0;
            var orderDishViewModels = new List<OrderDishViewModel>();

            if (reservation.OrderService?.OrderDishes != null)
            {
                foreach (var orderDish in reservation.OrderService.OrderDishes)
                {
                    decimal dishTotal = (orderDish.UnitPrice ?? 0) * (orderDish.Quantity ?? 0);
                    totalAmount += dishTotal;

                    orderDishViewModels.Add(new OrderDishViewModel
                    {
                        DishName = orderDish.Dish?.DishName ?? "Không xác định",
                        Quantity = orderDish.Quantity ?? 0,
                        UnitPrice = orderDish.UnitPrice ?? 0,
                        SubTotal = dishTotal,
                        Note = orderDish.Note
                    });
                }
            }

            string statusDisplay = GetStatusDisplay(reservation, reservation.OrderService);

            var viewModel = new CustomerReservationViewModel
            {
                Reservation = reservation,
                OrderService = reservation.OrderService,
                OrderDishes = orderDishViewModels,
                TotalAmount = totalAmount,
                StatusDisplay = statusDisplay
            };

            return View(viewModel);
        }

        private string GetStatusDisplay(TableReservation reservation, OrderService? orderService)
        {
            if (reservation.Status == ReservationStatus.Cancelled)
                return "Đã hủy";

            if (reservation.Status == ReservationStatus.Pending)
                return "Chờ xác nhận";

            if (reservation.Status == ReservationStatus.Confirmed)
            {
                DateTime now = DateTime.Now;

                if (now < reservation.StartTime)
                    return "Đã xác nhận - Chờ đến ngày";

                if (now >= reservation.StartTime && now <= reservation.EndTime)
                    return "Đang phục vụ";

                bool isPaid = orderService?.Payments?.Any() == true;
                if (!isPaid)
                    return "Chờ thanh toán";
            }

            if (reservation.Status == ReservationStatus.Completed)
                return "Hoàn thành";

            return "Đang xử lý";
        }
        // Add this method to the CustomerController class

        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.OrderService)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Kiểm tra trạng thái hiện tại của đặt bàn
            if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Confirmed)
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy đặt bàn khi trạng thái là 'Chờ xác nhận' hoặc 'Đã xác nhận'";
                return RedirectToAction("ReservationDetail", new { reservationId = id });
            }

            // Cập nhật trạng thái đặt bàn thành "Đã hủy"
            reservation.Status = ReservationStatus.Cancelled;

            // Cập nhật trạng thái bàn thành có sẵn
            if (reservation.Table != null)
            {
                reservation.Table.IsAvailable = true;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt bàn đã được hủy thành công";
            return RedirectToAction("MyReservations", new { customerId = reservation.CustomerId });
        }
    }
}
