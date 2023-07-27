using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            TempData["successMessage"] = $"Category {newCategory.Name} created successfuly"; 
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid) return View();

            _context.Categories.Update(category);
            _context.SaveChanges();
            TempData["successMessage"] = $"Category {category.Name} updated successfuly";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            _context.Remove(category);
            _context.SaveChanges(true);
            TempData["successMessage"] = $"Category {category.Name} deleted successfuly";

            return RedirectToAction("Index");
        }
    }
}
