using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CONST_Roles.Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.CategoryRepo.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category newCategory)
        {
            if (!ModelState.IsValid) return View();

            _unitOfWork.CategoryRepo.Add(newCategory);
            _unitOfWork.Save();
            TempData["successMessage"] = $"Category {newCategory.Name} created successfuly";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var category = _unitOfWork.CategoryRepo.Get(c => c.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid) return View();

            _unitOfWork.CategoryRepo.Update(category);
            _unitOfWork.Save();
            TempData["successMessage"] = $"Category {category.Name} updated successfuly";

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var category = _unitOfWork.CategoryRepo.Get(c => c.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var category = _unitOfWork.CategoryRepo.Get(c => c.Id == id);
            if (category == null) return NotFound();

            _unitOfWork.CategoryRepo.Delete(category);
            _unitOfWork.Save();
            TempData["successMessage"] = $"Category {category.Name} deleted successfuly";

            return RedirectToAction("Index");
        }
    }
}
