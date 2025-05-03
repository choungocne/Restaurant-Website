using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace RestaurantWebsite.Controllers
{
    public class ReservationController : Controller
    {
        private readonly RestaurantContext _context;
        private readonly IServiceProvider _serviceProvider;

        public ReservationController(RestaurantContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        // Hiển thị trang đặt lịch
        public IActionResult Create()
        {
            // Lấy danh sách bàn có sẵn
            ViewBag.AvailableTables = _context.DiningTables
                .Where(t => t.IsAvailable)
                .ToList();

            // Lấy danh sách món ăn theo danh mục
            ViewBag.Categories = _context.DishCategories
                .Include(c => c.Dishes)
                .ToList();

            return View(new TableReservation());
        }

        // Xử lý đặt bàn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TableReservation reservation, int numberOfCustomers, List<int> selectedDishIds, List<int> dishQuantities, string specialRequests)
        {
            // Loại bỏ các trường khỏi ModelState để tránh lỗi xác thực
            ModelState.Remove("TableId");
            ModelState.Remove("CustomerId");
            ModelState.Remove("Customer");
            ModelState.Remove("Table");

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTables = await _context.DiningTables
                    .Where(t => t.IsAvailable)
                    .ToListAsync();

                ViewBag.Categories = await _context.DishCategories
                    .Include(c => c.Dishes)
                    .ToListAsync();

                return View(reservation);
            }

            // Kiểm tra và tạo/cập nhật thông tin khách hàng
            Customer customer;
            if (Request.Form.ContainsKey("Customer.CustomerId"))
            {
                string name = Request.Form["Customer.FullName"].ToString();
                string phone = Request.Form["Customer.Phone"].ToString();

                customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerName == name);

                if (customer == null)
                {
                    // Tạo khách hàng mới
                    customer = new Customer
                    {
                        CustomerName = name,
                    
                        PhoneNumber = phone
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                // Gán CustomerId cho đơn đặt bàn
                reservation.CustomerId = customer.CustomerId;
            }
            else
            {
                ModelState.AddModelError("", "Thông tin khách hàng là bắt buộc");
                ViewBag.AvailableTables = await _context.DiningTables
                    .Where(t => t.IsAvailable)
                    .ToListAsync();
                ViewBag.Categories = await _context.DishCategories
                    .Include(c => c.Dishes)
                    .ToListAsync();
                return View(reservation);
            }

            // Kiểm tra bàn có tồn tại không
            var table = await _context.DiningTables.FindAsync(reservation.TableId);
            if (table == null)
            {
                ModelState.AddModelError("", "Không tìm thấy bàn");
                ViewBag.AvailableTables = await _context.DiningTables
                    .Where(t => t.IsAvailable)
                    .ToListAsync();
                ViewBag.Categories = await _context.DishCategories
                    .Include(c => c.Dishes)
                    .ToListAsync();
                return View(reservation);
            }

            if (numberOfCustomers > table.NumberOfCustomer)
            {
                ModelState.AddModelError("", $"Số lượng khách ({numberOfCustomers}) vượt quá sức chứa của bàn ({table.NumberOfCustomer})");
                ViewBag.AvailableTables = await _context.DiningTables
                    .Where(t => t.IsAvailable)
                    .ToListAsync();
                ViewBag.Categories = await _context.DishCategories
                    .Include(c => c.Dishes)
                    .ToListAsync();
                return View(reservation);
            }

            // Kiểm tra bàn có sẵn trong khoảng thời gian đã chọn
            var isAvailable = await CheckTableAvailabilityAsync(reservation.TableId, reservation.StartTime, reservation.EndTime);
            if (!isAvailable.isAvailable)
            {
                ModelState.AddModelError("", "Bàn này đã được đặt trong khoảng thời gian đã chọn");
                ViewBag.AvailableTables = await _context.DiningTables
                    .Where(t => t.IsAvailable)
                    .ToListAsync();
                ViewBag.Categories = await _context.DishCategories
                    .Include(c => c.Dishes)
                    .ToListAsync();
                return View(reservation);
            }

            // Tạo đơn đặt bàn
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Lưu thông tin đặt bàn
                    reservation.Status = ReservationStatus.Pending;
                    reservation.CreatedAt = DateTime.Now;
                    _context.TableReservations.Add(reservation);
                    await _context.SaveChangesAsync();

                    // Tạo dịch vụ đặt món
                    var orderService = new OrderService
                    {
                        TableId = reservation.TableId,
                        CustomerId = reservation.CustomerId,
                        StartTime = reservation.StartTime,
                        EndTime = reservation.EndTime,
                    };
                    _context.OrderServices.Add(orderService);
                    await _context.SaveChangesAsync();

                    // Liên kết OrderService với Reservation
                    reservation.OrderService = orderService;
                    await _context.SaveChangesAsync();

                    // Thêm món ăn vào đơn hàng
                    if (selectedDishIds != null && selectedDishIds.Count > 0)
                    {
                        for (int i = 0; i < selectedDishIds.Count; i++)
                        {
                            int dishId = selectedDishIds[i];
                            int quantity = i < dishQuantities.Count ? dishQuantities[i] : 1;

                            var dish = await _context.Dishes.FindAsync(dishId);
                            if (dish != null)
                            {
                                var orderDish = new OrderDish
                                {
                                    ServiceId = orderService.ServiceId,
                                    DishId = dishId,
                                    Quantity = quantity,
                                    UnitPrice = dish.UnitPrice
                                };
                                _context.OrderDishes.Add(orderDish);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Thiết lập hẹn giờ hủy nếu không thanh toán trong 30 phút
                    StartPaymentTimer(reservation.ReservationId);

                    // Commit transaction
                    await transaction.CommitAsync();

                    return RedirectToAction("Confirmation", new { id = reservation.ReservationId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"Lỗi khi đặt bàn: {ex.Message}");
                    ViewBag.AvailableTables = await _context.DiningTables
                        .Where(t => t.IsAvailable)
                        .ToListAsync();
                    ViewBag.Categories = await _context.DishCategories
                        .Include(c => c.Dishes)
                        .ToListAsync();
                    return View(reservation);
                }
            }
        }

        // API method to check availability
        [HttpGet]
        public async Task<JsonResult> CheckTableAvailability(int tableId, DateTime startTime, DateTime endTime)
        {
            var result = await CheckTableAvailabilityAsync(tableId, startTime, endTime);
            return Json(result);
        }

        // Trang xác nhận đặt bàn
        public async Task<IActionResult> Confirmation(int id)
        {
            var reservation = await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
                return NotFound();

            // Lấy thông tin các món đã đặt
            var orderService = await _context.OrderServices
                .Include(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.TableId == reservation.TableId &&
                                         o.CustomerId == reservation.CustomerId &&
                                         o.StartTime == reservation.StartTime);

            ViewBag.OrderService = orderService;

            return View(reservation);
        }

        // Payment page
        public async Task<IActionResult> Payment(int id)
        {
            var reservation = await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
                return NotFound();

            // Lấy thông tin các món đã đặt
            var orderService = await _context.OrderServices
                .Include(o => o.OrderDishes)
                .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.TableId == reservation.TableId &&
                                         o.CustomerId == reservation.CustomerId &&
                                         o.StartTime == reservation.StartTime);

            ViewBag.OrderService = orderService;

            return View(reservation);
        }

        // Process payment (this is just a placeholder - actual payment processing would be more complex)
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> ProcessPayment(int ReservationId, int ServiceId, decimal Amount, string PaymentMethod)
        //{
        //    var reservation = await _context.TableReservations.FindAsync(ReservationId);
        //    if (reservation == null)
        //        return NotFound();

        //    var orderService = await _context.OrderServices.FindAsync(ServiceId);
        //    if (orderService == null)
        //        return NotFound();

        //    // Create payment record
        //    var payment = new Payment
        //    {
        //        ServiceId = ServiceId,
        //        Amount = Amount,
        //        PaymentMethod = PaymentMethod,
        //        PaymentDate = DateTime.Now,
        //        Status = "Completed"
        //    };

        //    _context.Payments.Add(payment);

        //    // Update reservation status
        //    reservation.Status = ReservationStatus.Confirmed;

        //    await _context.SaveChangesAsync();

        //    // Redirect to a success page
        //    return RedirectToAction("PaymentSuccess", new { id = ReservationId });
        //}

        //// Payment success page
        //public async Task<IActionResult> PaymentSuccess(int id)
        //{
        //    var reservation = await _context.TableReservations
        //        .Include(r => r.Table)
        //        .Include(r => r.Customer)
        //        .FirstOrDefaultAsync(r => r.ReservationId == id);

        //    if (reservation == null)
        //        return NotFound();

        //    return View(reservation);
        //}

        // Helper method to check table availability
        private async Task<(bool isAvailable, string message)> CheckTableAvailabilityAsync(int tableId, DateTime startTime, DateTime endTime)
        {
            // Check if startTime is before endTime
            if (startTime >= endTime)
            {
                return (false, "Thời gian kết thúc phải sau thời gian bắt đầu");
            }

            // Check if day is Sunday
            if (startTime.DayOfWeek == DayOfWeek.Sunday || endTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return (false, "Không thể đặt bàn vào ngày Chủ Nhật");
            }

            // Check if time is between 17:00 and 23:00
            if (startTime.Hour < 17 || startTime.Hour > 23 || endTime.Hour < 17 || endTime.Hour > 23)
            {
                return (false, "Chỉ có thể đặt bàn từ 17:00 đến 23:00");
            }

            var overlappingReservations = await _context.TableReservations
                .Where(r => r.TableId == tableId &&
                           r.Status != ReservationStatus.Cancelled &&
                           ((r.StartTime <= startTime && r.EndTime > startTime) ||
                            (r.StartTime < endTime && r.EndTime >= endTime) ||
                            (r.StartTime >= startTime && r.EndTime <= endTime)))
                .ToListAsync();

            return (!overlappingReservations.Any(), overlappingReservations.Any() ?
                "Bàn này đã được đặt trong khoảng thời gian đã chọn" : "Bàn khả dụng");
        }

        // Hẹn giờ hủy đặt bàn nếu không thanh toán trong 30 phút
        private void StartPaymentTimer(int reservationId)
        {
            System.Threading.Timer timer = null;

            timer = new System.Threading.Timer(async state =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantContext>();
                    var reservation = await dbContext.TableReservations.FindAsync(reservationId);

                    if (reservation != null && reservation.Status == ReservationStatus.Pending)
                    {
                        // Kiểm tra xem đã thanh toán chưa
                        var orderService = await dbContext.OrderServices
                            .FirstOrDefaultAsync(o => o.TableId == reservation.TableId &&
                                                    o.CustomerId == reservation.CustomerId &&
                                                    o.StartTime == reservation.StartTime);

                        bool isPaid = false;
                        if (orderService != null)
                        {
                            isPaid = await dbContext.Payments
                                .AnyAsync(p => p.ServiceId == orderService.ServiceId);
                        }

                        if (!isPaid)
                        {
                            // Hủy đặt bàn
                            reservation.Status = ReservationStatus.Cancelled;

                            // Cập nhật trạng thái bàn
                            var table = await dbContext.DiningTables.FindAsync(reservation.TableId);
                            if (table != null)
                                table.IsAvailable = true;

                            await dbContext.SaveChangesAsync();
                        }
                    }

                    timer?.Dispose();
                }
            }, null, TimeSpan.FromMinutes(30), Timeout.InfiniteTimeSpan);
        }

        // Remaining controller methods...
    }
}