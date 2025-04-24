using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantWebsite.Models;
using RestaurantWebsite.Repository;

namespace RestaurantWebsite.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }
        public IActionResult Add()
        {
            var employees = employeeRepository.GetAll();
            ViewBag.Employees = new SelectList(employees, "EmployeeID","FullName","BirdthDate","PhoneNumber");
            return View();
        }
        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employeeRepository.Add(employee);
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Index()
        {
            var employees = employeeRepository.GetAll();
            return View(employees);
        }
        public IActionResult Display(int id)
        {
            var employees = employeeRepository.GetById(id);
            if (employees == null)
            {
                return NotFound();
            }
            return View(employees);
        }
        public IActionResult Update(int id)
        {
            var employees = employeeRepository.GetById(id);
            if (employees == null)
            {
                return NotFound();
            }
            return View(employees);
        }
        [HttpPost]
        public IActionResult Update(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employeeRepository.Update(employee);
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            var employees = employeeRepository.GetById(id);
            if (employees == null)
            {
                return NotFound();
            }
            return View(employees);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
           
                employeeRepository.Delete(id);
                return RedirectToAction("Index");
           
        }
    }
    
}
