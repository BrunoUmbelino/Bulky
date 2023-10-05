using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var userId = (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is not null)
            {
                var quantityCartItems = _unitOfWork.ShoppingCartRepo.GetAll(s => s.ApplicationUserId == userId)?.Count();
                if (quantityCartItems is not null) HttpContext.Session.SetInt32(CONST_Session.ShoppingCart, (int)quantityCartItems);
            }

            var products = _unitOfWork.ProductRepo.GetAll(
                includeProperties: $"{nameof(Product.Category)}, {nameof(Product.Images)}");
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            if (productId == 0) return NotFound();
            var product = _unitOfWork.ProductRepo.Get(p => p.Id == productId, 
                includeProperties: $"{nameof(Product.Category)}, {nameof(Product.Images)}");
            if (product == null) return NotFound();

            ShoppingCartItem shoppingCart = new()
            {
                Product = product,
                ProductId = productId,
                Count = 1
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCartItem shoppingCart)
        {
            try
            {
                if (shoppingCart.Count <= 0 || shoppingCart.ProductId == 0) return ValidationProblem();

                string? userIdFromIdentity = (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                shoppingCart.ApplicationUserId = userIdFromIdentity;

                ShoppingCartItem shoppingCartFromDb = _unitOfWork.ShoppingCartRepo
                    .Get(sc => sc.ApplicationUserId == userIdFromIdentity && sc.ProductId == shoppingCart.ProductId);

                if (shoppingCartFromDb == null)
                {
                    _unitOfWork.ShoppingCartRepo.Add(shoppingCart);
                    _unitOfWork.Save();
                    var cartItemsQuantity = _unitOfWork.ShoppingCartRepo.GetAll(s => s.ApplicationUserId == userIdFromIdentity)?.Count();
                    if (cartItemsQuantity is not null)
                        HttpContext.Session.SetInt32(CONST_Session.ShoppingCart, (int)cartItemsQuantity);

                }
                else
                {
                    shoppingCartFromDb.Count += shoppingCart.Count;
                    _unitOfWork.ShoppingCartRepo.Update(shoppingCartFromDb);
                    _unitOfWork.Save();
                }

                TempData["successMessage"] = $"Cart updated sucessfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Erro no processo de UPSERT do item carrinho de compras.");
                TempData["errorMessage"] = $"Something went wrong but don't be sad, it wasn't you fault.";
                return RedirectToAction(nameof(Index));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}