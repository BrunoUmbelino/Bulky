using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        #region ACTIONS

        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository
                .GetAll(includeProperties: "Category");
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductVM()
            {
                CategoryList = PopulateCategoryList()
            };

            if (id == null || id == 0) return View(productVM);

            productVM.Product = _unitOfWork.ProductRepository.Get(p => p.Id == id);

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (productVM.Product.ImageUrl == null) productVM.Product.ImageUrl = "";

            if (!ModelState.IsValid)
            {
                productVM.CategoryList = PopulateCategoryList();
                return View(productVM);
            }

            if (file != null)
            {
                var wwwRootPath = _webHostEnvironment.WebRootPath;

                if (!productVM.Product.ImageUrl.IsNullOrEmpty())
                {
                    DeleteProductImageIfExist(productVM.Product.ImageUrl, wwwRootPath);
                }

                var newImageName = SaveProductImage(file, wwwRootPath);
                productVM.Product.ImageUrl = @$"\images\products\{newImageName}";
            }

            string actionMessage;
            if (productVM.Product.Id == 0)
            {
                _unitOfWork.ProductRepository.Add(productVM.Product);
                actionMessage = "created";
            }
            else
            {
                _unitOfWork.ProductRepository.Update(productVM.Product);
                actionMessage = "updated";
            }

            _unitOfWork.Save();
            TempData["successMessage"] = $"Product {productVM.Product.Title} {actionMessage} successfuly";

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            var product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            var product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
            if (product == null) return NotFound();

            _unitOfWork.ProductRepository.Delete(product);
            _unitOfWork.Save();
            TempData["successMessage"] = $"Product {product.Title} removed successfuly";

            return RedirectToAction("Index");
        }

        #endregion

        #region API CALLS

        [HttpGet]
        [Route("[area]/API/[controller]/GetAll")]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.ProductRepository
                .GetAll(includeProperties: "Category");
            return Json(new { success = true, data = products });
        }


        [Route("[area]/API/[controller]/Delete")]
        public IActionResult DeleteApi(int? id)
        {
            var product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var wwwRootPath = _webHostEnvironment.WebRootPath;
            DeleteProductImageIfExist(wwwRootPath, product.ImageUrl);

            _unitOfWork.ProductRepository.Delete(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion

        #region AUXILIARY FUNCTIONS 

        IEnumerable<SelectListItem> PopulateCategoryList()
        {
            return _unitOfWork.CategoryRepository
                .GetAll()
                .Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
        }

        private static string SaveProductImage(IFormFile file, string wwwRootPath)
        {
            var productImagesPath = Path.Combine(wwwRootPath, @"images\products");
            var newImageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(productImagesPath, newImageName);
            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                file.CopyTo(fileStream);
            }

            return newImageName;
        }

        static void DeleteProductImageIfExist(string imageUrl, string wwwRootPath)
        {
            var oldImagePath = Path.Combine(wwwRootPath, imageUrl.Trim('\\'));
            if (System.IO.File.Exists(oldImagePath)) 
                System.IO.File.Delete(oldImagePath);
        }

        #endregion
    }
}
