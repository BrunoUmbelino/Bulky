using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    [Authorize(Roles = CONST_Roles.Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }


        #region ACTIONS

        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepo
                .GetAll(includeProperties: "Category");
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductVM()
            {
                CategoryList = PopulateCategoryList()
            };

            if (id.HasValue)
            {
                productVM = (_unitOfWork.ProductRepo.Get(p => p.Id == id, includeProperties: $"{nameof(Product.Images)}")as IQueryable)
                    .ProjectTo<ProductVM>(_mapper.ConfigurationProvider).FirstOrDefault();
                return View(productVM);
            }

            return View(productVM);
        }

        //[HttpPost]
        //public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        productVM.CategoryList = PopulateCategoryList();
        //        return View(productVM);
        //    }

        //    string actionMessage;
        //    if (productVM.Id == 0)
        //    {
        //        _unitOfWork.ProductRepo.Add(productVM);
        //        actionMessage = "created";
        //    }
        //    else
        //    {
        //        _unitOfWork.ProductRepo.Update(productVM);
        //        actionMessage = "updated";
        //    }
        //    _unitOfWork.Save();

        //    if (files != null) SaveProductImagesAndFiles(files, productVM);


        //    _unitOfWork.Save();
        //    TempData["successMessage"] = $"Product {productVM.Title} {actionMessage} successfuly";

        //    return RedirectToAction("Index");
        //}

        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            var product = _unitOfWork.ProductRepo.Get(p => p.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            var product = _unitOfWork.ProductRepo.Get(p => p.Id == id);
            if (product == null) return NotFound();



            _unitOfWork.ProductRepo.Delete(product);
            _unitOfWork.Save();
            TempData["successMessage"] = $"Product {product.Title} removed successfuly";
            return RedirectToAction("Index");
        }

        public IActionResult DeleteImage(int imageId)
        {
            var image = _unitOfWork.ProductImageRepo.Get(i => i.Id == imageId);
            if (image == null || image.ImageUrl.IsNullOrEmpty())
            {
                TempData["errorMessage"] = "Resource not found";
                return RedirectToAction(nameof(Index));
            }

            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var oldImagePath = Path.Combine(wwwRootPath, image.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);

            _unitOfWork.ProductImageRepo.Delete(image);
            _unitOfWork.Save();

            TempData["successMessage"] = $"Image removed successfuly";
            return RedirectToAction(nameof(Upsert), new { id = image.ProductId });
        }

        #endregion


        #region API's

        [HttpGet]
        [Route("[area]/API/[controller]/GetAll")]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.ProductRepo
                .GetAll(includeProperties: "Category");
            return Json(new { success = true, data = products });
        }

        [Route("[area]/API/[controller]/Delete")]
        public IActionResult DeleteApi(int? id)
        {
            var product = _unitOfWork.ProductRepo.Get(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @$"images\products\product-{product.Id}";
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            if (!Directory.Exists(finalPath)) return NotFound();

            string[] filePaths = Directory.GetFiles(finalPath);
            foreach (string filePath in filePaths)
            {
                System.IO.File.Delete(filePath);
            }
            Directory.Delete(finalPath);

            _unitOfWork.ProductRepo.Delete(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = $"Product {product.Title} deleted successfully." });
        }

        #endregion


        #region PRIVATE FUNCTIONS 

        private IEnumerable<SelectListItem> PopulateCategoryList()
        {
            return _unitOfWork.CategoryRepo
                .GetAll()
                .Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
        }

        private void SaveProductImagesAndFiles(List<IFormFile> files, ProductVM product)
        {
            var wwwRootPath = _webHostEnvironment.WebRootPath;

            foreach (var file in files)
            {
                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = @$"images\products\product-{product.Id}";
                string finalPath = Path.Combine(wwwRootPath, productPath);

                if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);
                using (var fileStream = new FileStream(Path.Combine(finalPath, newFileName), FileMode.CreateNew))
                {
                    file.CopyTo(fileStream);
                }

                product.Images.Add(new ProductImage()
                {
                    ImageUrl = @$"\{productPath}\{newFileName}",
                    ProductId = product.Id ?? 0
                }) ; ;

                //_unitOfWork.ProductRepo.Update(product);
                _unitOfWork.Save();
            }
        }

        #endregion
    }
}
