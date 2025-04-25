using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using RestaurantWebsite.Models;
using RestaurantWebsite.Repository;

namespace RestaurantWebsite.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(IEmployeeRepository employeeRepository, IWebHostEnvironment webHostEnvironment)
        {
            _employeeRepository = employeeRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            try
            {
                // Get all employees
                var employees = _employeeRepository.GetAll();

                // Add debug information
                ViewBag.EmployeeCount = employees.Count();
                ViewBag.HasData = employees.Any();

                return View(employees);
            }
            catch (Exception ex)
            {
                // Handle and display any exceptions
                ViewBag.ErrorMessage = $"Error retrieving employee data: {ex.Message}";
                ViewBag.StackTrace = ex.StackTrace;
                return View(new List<Employee>());
            }
        }

        public IActionResult Display(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Employee employee, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image file upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Ensure directory exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            ImageFile.CopyTo(fileStream);
                        }

                        employee.Img = uniqueFileName;
                    }

                    _employeeRepository.Add(employee);
                    TempData["SuccessMessage"] = "Employee added successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error adding employee: {ex.Message}");
                }
            }
            return View(employee);
        }

        public IActionResult Update(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        public IActionResult Update(Employee employee, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image file upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Ensure directory exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            ImageFile.CopyTo(fileStream);
                        }

                        // Delete old image if exists
                        var existingEmployee = _employeeRepository.GetById(employee.EmployeeId);
                        if (existingEmployee != null && !string.IsNullOrEmpty(existingEmployee.Img))
                        {
                            string oldFilePath = Path.Combine(uploadsFolder, existingEmployee.Img);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        employee.Img = uniqueFileName;
                    }
                    else
                    {
                        // Keep existing image if no new one was uploaded
                        var existingEmployee = _employeeRepository.GetById(employee.EmployeeId);
                        if (existingEmployee != null)
                        {
                            employee.Img = existingEmployee.Img;
                        }
                    }

                    _employeeRepository.Update(employee);
                    TempData["SuccessMessage"] = "Employee updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating employee: {ex.Message}");
                }
            }
            return View(employee);
        }

        public IActionResult Delete(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var employee = _employeeRepository.GetById(id);
                if (employee != null && !string.IsNullOrEmpty(employee.Img))
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string filePath = Path.Combine(uploadsFolder, employee.Img);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _employeeRepository.Delete(id);
                TempData["SuccessMessage"] = "Employee deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting employee: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}