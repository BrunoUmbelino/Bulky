using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
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
            var products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            if (productId == 0) return NotFound();
            var product = _unitOfWork.ProductRepository.Get(p=>p.Id== productId, includeProperties: "Category");
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

                ShoppingCartItem shoppingCartFromDb = _unitOfWork.ShoppingCartRepository
                    .Get(sc => sc.ApplicationUserId == userIdFromIdentity && sc.ProductId == shoppingCart.ProductId);

                if (shoppingCartFromDb == null)
                {
                    _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                }
                else
                {
                    shoppingCartFromDb.Count += shoppingCart.Count;
                    _unitOfWork.ShoppingCartRepository.Update(shoppingCartFromDb);
                }
                _unitOfWork.Save();

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}