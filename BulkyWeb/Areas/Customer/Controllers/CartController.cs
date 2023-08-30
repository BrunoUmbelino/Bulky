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
            var userId = (User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shopCartVM = new()
            {
                ShoppingCartItems = _unitOfWork.ShoppingCartRepository.GetAll(
                    filter: sc=>sc.ApplicationUserId == userId, 
                    includeProperties: "Product"),
            };

            foreach(var cart in shopCartVM.ShoppingCartItems)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shopCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            
            return View(shopCartVM);
        }

        public IActionResult PlusCartItem(int cartItemId)
        {
            if (cartItemId == 0) return NotFound();
            var cartItemDB = _unitOfWork.ShoppingCartRepository.Get(sc=>sc.Id == cartItemId, includeProperties: "Product");
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
            } else
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
            return View();
        }

        private double GetPriceBasedOnQuantity(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem.Count <= 50) return shoppingCartItem.Product.PriceUp50;
            else if (shoppingCartItem.Count <= 100) return shoppingCartItem.Product.PriceUp100;
            else if (shoppingCartItem.Count > 100) return shoppingCartItem.Product.PriceAbove100;
            else return 0;
        }
    }
}
