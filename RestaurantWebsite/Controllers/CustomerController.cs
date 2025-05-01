using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RestaurantWebsite.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

            // Send both customer and user account to the view
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

                    // Redirect to the profile page after successful update
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the error
                    ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);

                    // If there's an error, return to the edit form
                    // We need to send the user account data again
                    ViewBag.UserAccount = _context.UserAccounts.FirstOrDefault(u => u.CustomerId == id);
                    return View(customer);
                }
            }

            // If model state is invalid, repopulate the ViewBag and return to the view
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
    }
}