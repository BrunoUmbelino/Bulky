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
        public IActionResult Details(ShoppingCartItem newShoppingCart)
        {
            try
            {
                if (newShoppingCart.Count <= 0 || newShoppingCart.ProductId == 0) return ValidationProblem();

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                newShoppingCart.ApplicationUserId = userId;

                ShoppingCartItem shoppingCartFromDb = _unitOfWork.ShoppingCartRepository
                    .Get(sc => sc.ApplicationUserId == userId && sc.ProductId == newShoppingCart.ProductId);

                string messageAction = "";
                if (shoppingCartFromDb == null)
                {
                    messageAction = "created";
                    _unitOfWork.ShoppingCartRepository.Add(newShoppingCart);
                }
                else
                {
                    messageAction = "updated";
                    shoppingCartFromDb.Count += newShoppingCart.Count;
                    _unitOfWork.ShoppingCartRepository.Update(shoppingCartFromDb);
                }
                _unitOfWork.Save();

                TempData["successMessage"] = $"Cart {messageAction} sucessfully";
                return RedirectToAction(nameof(Index));
            } 
            catch (Exception ex)
            {
                _logger.LogError("", ex);
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