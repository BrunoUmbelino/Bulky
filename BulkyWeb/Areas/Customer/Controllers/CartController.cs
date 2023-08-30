using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult Index()
        {
            string? userId = (User.Identity as ClaimsIdentity)?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoppingCartVM shopCartVM = new()
            {
                ShoppingCartItems = _unitOfWork.ShoppingCartRepository.GetAll(
                    filter: sc => sc.ApplicationUserId == userId,
                    includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in shopCartVM.ShoppingCartItems)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shopCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shopCartVM);
        }

        public IActionResult PlusCartItem(int cartItemId)
        {
            if (cartItemId == 0) return NotFound();
            var cartItemDB = _unitOfWork.ShoppingCartRepository.Get(sc => sc.Id == cartItemId, includeProperties: "Product");
            if (cartItemDB == null) return NotFound();

            cartItemDB.Count += 1;
            cartItemDB.Price = GetPriceBasedOnQuantity(cartItemDB);

            _unitOfWork.ShoppingCartRepository.Update(cartItemDB);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult MinusCartItem(int cartItemId)
        {
            if (cartItemId == 0) return NotFound();
            var cartItemDB = _unitOfWork.ShoppingCartRepository.Get(sc => sc.Id == cartItemId, includeProperties: "Product");
            if (cartItemDB == null) return NotFound();

            cartItemDB.Count -= 1;
            if (cartItemDB.Count <= 0)
            {
                _unitOfWork.ShoppingCartRepository.Delete(cartItemDB);
            }
            else
            {
                cartItemDB.Price = GetPriceBasedOnQuantity(cartItemDB);
                _unitOfWork.ShoppingCartRepository.Update(cartItemDB);
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCartItem(int cartItemId)
        {
            if (cartItemId == 0) return NotFound();
            var cartItemDB = _unitOfWork.ShoppingCartRepository.Get(sc => sc.Id == cartItemId, includeProperties: "Product");
            if (cartItemDB == null) return NotFound();

            _unitOfWork.ShoppingCartRepository.Delete(cartItemDB);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            string? userId = (User.Identity as ClaimsIdentity)?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userFromDb = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);
            if (userFromDb == null) return NotFound();

            ShoppingCartVM shopCartVM = new()
            {
                ShoppingCartItems = _unitOfWork.ShoppingCartRepository
                    .GetAll(filter: sc => sc.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
                {
                    ApplicationUser = userFromDb,
                    PhoneNumber = userFromDb.PhoneNumber ?? string.Empty,
                    StreetAddress = userFromDb.StreetAddress ?? string.Empty,
                    City = userFromDb.City ?? string.Empty,
                    State = userFromDb.State ?? string.Empty,
                    PostalCode = userFromDb.PostalCode ?? string.Empty,
                    Name = userFromDb.Name ?? string.Empty,
                }
            };

            foreach (var cart in shopCartVM.ShoppingCartItems)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shopCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shopCartVM);
        }

        private double GetPriceBasedOnQuantity(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem.Product is null) throw new ArgumentException("Invalid Product");

            if (shoppingCartItem.Count <= 50) return shoppingCartItem.Product.PriceUp50;
            else if (shoppingCartItem.Count <= 100) return shoppingCartItem.Product.PriceUp100;
            else if (shoppingCartItem.Count > 100) return shoppingCartItem.Product.PriceAbove100;
            else return 0;
        }
    }
}
