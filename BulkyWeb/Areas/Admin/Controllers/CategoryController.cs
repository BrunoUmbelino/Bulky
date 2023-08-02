using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.CateroryRepository.GetAll();
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

            _unitOfWork.CateroryRepository.Add(newCategory);
            _unitOfWork.Save();
            TempData["successMessage"] = $"Category {newCategory.Name} created successfuly";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var category = _unitOfWork.CateroryRepository.Get(c => c.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid) return View();

            _unitOfWork.CateroryRepository.Update(category);
            _unitOfWork.Save();
            TempData["successMessage"] = $"Category {category.Name} updated successfuly";

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var category = _unitOfWork.CateroryRepository.Get(c => c.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var category = _unitOfWork.CateroryRepository.Get(c => c.Id == id);
            if (category == null) return NotFound();

            _unitOfWork.CateroryRepository.Delete(category);
            _unitOfWork.Save();
            TempData["successMessage"] = $"Category {category.Name} deleted successfuly";

            return RedirectToAction("Index");
        }
    }
}
