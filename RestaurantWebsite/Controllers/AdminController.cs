using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using RestaurantWebsite.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantWebsite.Controllers
{
    [Authorize (Roles = "Admin,Employee")]
    public class AdminController : Controller
    {
        private readonly RestaurantContext _context;

        public AdminController(RestaurantContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new AdminDashboardViewModel
            {
                CustomerCount = await _context.Customers.CountAsync(),
                DishCount = await _context.Dishes.CountAsync(),
                ReservationCount = await _context.TableReservations.CountAsync(),
                EmployeeCount = await _context.Employees.CountAsync(),
                RecentReservations = await _context.TableReservations
                    .Include(r => r.Customer)
                    .Include(r => r.Table)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardViewModel);
        }
        [Authorize(Roles = "Admin")]

        public IActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(AdminRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.UserAccounts.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã được sử dụng");
                    return View(model);
                }

                if (await _context.UserAccounts.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được đăng ký");
                    return View(model);
                }

                // Create new employee
                var employee = new Employee
                {
                    FullName = model.FullName,
                    BirthDate = model.BirthDate,
                    PhoneNumber = model.PhoneNumber
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Create user account with Employee role
                var userAccount = new UserAccount
                {
                    Username = model.Username,
                    PasswordHash = HashPassword(model.Password),
                    Email = model.Email,
                    Role = "Employee",
                    CreatedAt = DateTime.Now,
                    EmployeeId = employee.EmployeeId
                };

                _context.UserAccounts.Add(userAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Employees));
            }
            return View(model);
        }

       
        public async Task<IActionResult> Employees()
        {
            var employees = await _context.Employees
                .Include(e => e.UserAccounts)
                .ToListAsync();
            return View(employees);
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.UserAccounts)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            var viewModel = new EditEmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                BirthDate = employee.BirthDate,
                PhoneNumber = employee.PhoneNumber,
                Detail = employee.Detail
            };

            if (employee.UserAccounts.Any())
            {
                var userAccount = employee.UserAccounts.First();
                viewModel.Email = userAccount.Email;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(int id, EditEmployeeViewModel model)
        {
            if (id != model.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var employee = await _context.Employees
                        .Include(e => e.UserAccounts)
                        .FirstOrDefaultAsync(e => e.EmployeeId == id);

                    if (employee == null)
                    {
                        return NotFound();
                    }

                    // Update employee information
                    employee.FullName = model.FullName;
                    employee.BirthDate = model.BirthDate;
                    employee.PhoneNumber = model.PhoneNumber;
                    employee.Detail = model.Detail;


                    if (employee.UserAccounts.Any())
                    {
                        var userAccount = employee.UserAccounts.First();
                        userAccount.Email = model.Email;
                        userAccount.Role = model.Role; // Cập nhật vai trò


                        // Update password if specified
                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            userAccount.PasswordHash = HashPassword(model.Password);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Employees));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(model.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.UserAccounts)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }
        [HttpPost, ActionName("DeleteEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.UserAccounts)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee != null)
            {
                _context.UserAccounts.RemoveRange(employee.UserAccounts); // xóa account trước
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Employees));
        }

        // GET: Admin/CreateAdmin
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // POST: Admin/CreateAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(AdminRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.UserAccounts.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã được sử dụng");
                    return View(model);
                }

                if (await _context.UserAccounts.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được đăng ký");
                    return View(model);
                }

                // Create new employee for admin
                var employee = new Employee
                {
                    FullName = model.FullName,
                    BirthDate = model.BirthDate,
                    PhoneNumber = model.PhoneNumber
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Create admin user account
                var userAccount = new UserAccount
                {
                    Username = model.Username,
                    PasswordHash = HashPassword(model.Password),
                    Email = model.Email,
                    Role = "Admin",
                    CreatedAt = DateTime.Now,
                    EmployeeId = employee.EmployeeId
                };

                _context.UserAccounts.Add(userAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // Manage dishes
        public async Task<IActionResult> Dishes()
        {
            var dishes = await _context.Dishes
                .Include(d => d.Category)
                .ToListAsync();
            return View(dishes);
        }
        [Authorize(Roles = "Admin")]

        public IActionResult CreateDish()
        {
            ViewBag.Categories = new SelectList(_context.DishCategories, "CategoryId", "CategoryName");
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDish(Dish model)
        {
            if (!ModelState.IsValid) return View(model);

            var dish = new Dish
            {
                DishName = model.DishName,
                UnitPrice = model.UnitPrice,
                Img = model.Img,
                CreatedAt = DateTime.Now,
                CategoryId = model.CategoryId
            };

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dishes");
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            var model = new Dish
            {
                DishId = dish.DishId,
                DishName = dish.DishName,
                UnitPrice = dish.UnitPrice,
                Img = dish.Img,
                CreatedAt = dish.CreatedAt,
                CategoryId = dish.CategoryId
            };

            ViewBag.Categories = new SelectList(_context.DishCategories, "CategoryId", "CategoryName", model.CategoryId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditDish(Dish model)
        {
            if (!ModelState.IsValid) return View(model);

            var dish = await _context.Dishes.FindAsync(model.DishId);
            if (dish == null) return NotFound();

            dish.DishName = model.DishName;
            dish.UnitPrice = model.UnitPrice;
            dish.Img = model.Img;
            dish.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction("Dishes");
        }
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            return View(dish);
        }

        [HttpPost, ActionName("DeleteDish")]
        public async Task<IActionResult> DeleteDis(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dishes");
        }

        public async Task<IActionResult> Customers()
        {
            var customers = await _context.Customers
                .Include(c => c.UserAccounts)
                .ToListAsync();
            return View(customers);
        }
        
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteCus(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Customers");
        }

        // Manage reservations
        public async Task<IActionResult> Reservations()
        {
            var reservations = await _context.TableReservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Include(r => r.Employee)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(reservations);
        }
        public async Task<IActionResult> EditReservation(int id)
        {
            var res = await _context.TableReservations.FindAsync(id);
            if (res == null) return NotFound();

            var model = new TableReservation
            {
                ReservationId = res.ReservationId,
                CustomerId = res.CustomerId,
                TableId = res.TableId,
                ServiceId = res.ServiceId,
                StartTime = res.StartTime,
                EndTime = res.EndTime,
                EmployeeId = res.EmployeeId,
                Status = (ReservationStatus)(int)res.Status
            };

            ViewBag.Tables = new SelectList(_context.DiningTables, "TableId", "TableName", model.TableId);
            ViewBag.Employees = new SelectList(_context.Employees, "EmployeeId", "FullName", model.EmployeeId);
            ViewBag.Statuses = Enum.GetValues(typeof(ReservationStatus)).Cast<ReservationStatus>()
                                   .Select(s => new SelectListItem { Value = ((int)s).ToString(), Text = s.ToString() }).ToList();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditReservation(TableReservation model)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await _context.TableReservations.FindAsync(model.ReservationId);
            if (res == null) return NotFound();

            res.TableId = model.TableId;
            res.ServiceId = model.ServiceId;
            res.StartTime = model.StartTime;
            res.EndTime = model.EndTime;
            res.EmployeeId = model.EmployeeId;
            res.Status = (ReservationStatus)model.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction("Reservations");
        }
        public async Task<IActionResult> Feedbacks ()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Reservation)
                .ThenInclude(r => r.Table)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(feedbacks);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Reservation)
                .ThenInclude(r => r.Table)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}