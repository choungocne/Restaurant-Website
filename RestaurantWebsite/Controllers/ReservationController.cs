using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RestaurantWebsite.ViewModels;

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
            ViewBag.Dishes = _context.Dishes.ToList();

            return View(new TableReservation());
        }

        // Xử lý đặt bàn
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create(TableReservation reservation, List<int> dishIds, List<int> quantities)
        {
            if (ModelState.ContainsKey("TableId"))
                ModelState["TableId"].Errors.Clear();

            if (ModelState.ContainsKey("CustomerId"))
                ModelState["CustomerId"].Errors.Clear();

            if (ModelState.IsValid)
            {
                if (reservation.TableId == 0 || reservation.CustomerId == 0)
                {
                    ModelState.AddModelError(string.Empty, "Bạn cần chọn khách hàng và bàn trước khi đặt lịch.");
                    return View(reservation);
                }

                // 1. Tạo OrderService gắn với lịch này
                var orderService = new OrderService
                {
                    CustomerId = reservation.CustomerId,
                    TableId = reservation.TableId,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime
                };
                _context.OrderServices.Add(orderService);
                await _context.SaveChangesAsync();

                // 2. Tạo danh sách OrderDish (từ dishIds & quantities)
                for (int i = 0; i < dishIds.Count; i++)
                {
                    var dish = await _context.Dishes.FindAsync(dishIds[i]);
                    if (dish == null) continue;

                    var orderDish = new OrderDish
                    {
                        ServiceId = orderService.ServiceId,
                        DishId = dishIds[i],
                        Quantity = quantities[i],
                        UnitPrice = dish.UnitPrice
                    };

                    _context.OrderDishes.Add(orderDish);
                }

                // 3. Gắn OrderService vào TableReservation
                reservation.ServiceId = orderService.ServiceId;
                _context.TableReservations.Add(reservation);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã đặt lịch thành công!";
                return RedirectToAction("Index", "Home");
            }

            return View(reservation);
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