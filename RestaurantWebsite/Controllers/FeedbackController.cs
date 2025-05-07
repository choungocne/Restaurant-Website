// FeedbackController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsite.Models;
using RestaurantWebsite.ViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection;
using System.Threading;

namespace RestaurantWebsite.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly RestaurantContext _context;

        public FeedbackController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: Customer feedback form
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(int reservationId)
        {
            var reservation = await _context.TableReservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            // Check if the reservation is completed
            if (reservation.Status != ReservationStatus.Completed)
            {
                TempData["ErrorMessage"] = "Bạn chỉ có thể đánh giá sau khi hoàn thành đặt bàn.";
                return RedirectToAction("MyReservations", "CustomerProfile");
            }

            var existingFeedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.CustomerId == reservation.CustomerId && f.ReservationId == reservationId);

            if (existingFeedback != null)
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá cho lượt đặt bàn này rồi.";
                return RedirectToAction("MyReservations", "CustomerProfile");
            }

            var model = new FeedbackViewModel
            {
                ReservationId = reservationId,
                CustomerId = reservation.CustomerId,
                ReservationDate = reservation.StartTime,
                TableName = reservation.Table.TableName
            };

            return View(model);
        }

        // POST: Submit feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                var feedback = new Feedback
                {
                    CustomerId = model.CustomerId,
                    ReservationId = model.ReservationId,
                    Rating = model.Rating,
                    Content = model.Content,
                    CreatedAt = DateTime.Now
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá!";
                return RedirectToAction("MyReservations", "CustomerProfile");
            }

            return View(model);
        }

       
       
    }
}
