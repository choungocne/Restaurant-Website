using Microsoft.AspNetCore.Mvc;
using RestaurantWebsite.Models;
using RestaurantWebsite.Repository;

namespace RestaurantWebsite.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _repo;

        public CustomerController(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index() => View(_repo.GetAll());

        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public IActionResult Update(int id)
        {
            var customer = _repo.GetById(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        public IActionResult Update(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public IActionResult Delete(int id)
        {
            var customer = _repo.GetById(id);
            return customer == null ? NotFound() : View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }

        public IActionResult Display(int id)
        {
            var customer = _repo.GetById(id);
            return customer == null ? NotFound() : View(customer);
        }
    }
}
