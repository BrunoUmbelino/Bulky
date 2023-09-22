using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartController> _logger;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; } = null!;

        public CartController(IUnitOfWork unitOfWork, ILogger<CartController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region ACTIONS


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
                _unitOfWork.Save();

                var cartItemsQuantity = _unitOfWork.ShoppingCartRepository.GetAll(s => s.ApplicationUserId == cartItemDB.ApplicationUserId)?.Count();
                if (cartItemsQuantity is not null)
                    HttpContext.Session.SetInt32(CONST_Session.ShoppingCart, (int)cartItemsQuantity);
            }
            else
            {
                cartItemDB.Price = GetPriceBasedOnQuantity(cartItemDB);
                _unitOfWork.ShoppingCartRepository.Update(cartItemDB);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCartItem(int cartItemId)
        {
            if (cartItemId == 0) return NotFound();
            var cartItemDB = _unitOfWork.ShoppingCartRepository.Get(sc => sc.Id == cartItemId, includeProperties: "Product");
            if (cartItemDB == null) return NotFound();

            _unitOfWork.ShoppingCartRepository.Delete(cartItemDB);
            _unitOfWork.Save();

            var cartItemsQuantity = _unitOfWork.ShoppingCartRepository.GetAll(s => s.ApplicationUserId == cartItemDB.ApplicationUserId)?.Count();
            if (cartItemsQuantity is not null)
                HttpContext.Session.SetInt32(CONST_Session.ShoppingCart, (int)cartItemsQuantity);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            string? userId = (User.Identity as ClaimsIdentity)?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound();

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
            shopCartVM.OrderHeader.OrderTotal = CalculateOrderTotal(shopCartVM);

            return View(shopCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ShoppingCartVM shopCartVM)
        {
            try
            {
                string? userId = (User.Identity as ClaimsIdentity)?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound();

                shopCartVM.ShoppingCartItems = _unitOfWork.ShoppingCartRepository
                    .GetAll(filter: sc => sc.ApplicationUserId == userId, includeProperties: "Product");
                shopCartVM.OrderHeader.ApplicationUserId = userId;
                shopCartVM.OrderHeader.OrderDate = DateTime.Now;
                shopCartVM.OrderHeader.OrderTotal = CalculateOrderTotal(shopCartVM);

                var applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);
                if (applicationUser is null) return NotFound();

                bool isCompanyAccout = applicationUser.CompanyId.GetValueOrDefault() != 0;
                bool isCustomerAccout = !isCompanyAccout;
                if (isCustomerAccout)
                {
                    shopCartVM.OrderHeader.PaymentStatus = CONST_PaymentStatus.Pending;
                    shopCartVM.OrderHeader.OrderStatus = CONST_OrderStatus.Pending;
                }
                if (isCompanyAccout)
                {
                    shopCartVM.OrderHeader.PaymentStatus = CONST_PaymentStatus.DelayedPayment;
                    shopCartVM.OrderHeader.OrderStatus = CONST_PaymentStatus.Approved;
                }
                _unitOfWork.OrderHeaderRepository.Add(shopCartVM.OrderHeader);
                _unitOfWork.Save();

                foreach (var item in shopCartVM.ShoppingCartItems)
                {
                    OrderDetail orderDetail = new()
                    {
                        ProductId = item.ProductId,
                        OrderHeaderId = shopCartVM.OrderHeader.Id,
                        Price = item.Price,
                        Count = item.Count,
                    };
                    _unitOfWork.OrderDetailRepository.Add(orderDetail);
                    _unitOfWork.Save();
                }

                if (isCustomerAccout)
                {
                    var session = PaymentForStripe(shopCartVM);
                    Response.Headers.Location = session.Url;
                    return new StatusCodeResult(303);
                }

                return RedirectToAction(nameof(OrderConfirmation), new { id = shopCartVM.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Erro na criação de Ordem.");
                TempData["errorMessage"] = $"Something went wrong but don't be sad, it wasn't you fault.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.Get(o => o.Id == id);
            if (orderHeader == null) return NotFound();

            var isOrderByCompany = orderHeader.PaymentStatus == CONST_PaymentStatus.DelayedPayment;
            if (isOrderByCompany)
            {
                var shoppingCartsForRemove = _unitOfWork.ShoppingCartRepository
                    .GetAll(s => s.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCartRepository.DeleteRange(shoppingCartsForRemove);
                _unitOfWork.Save();

                return View(id);
            }

            var stripeService = new SessionService();
            Session stripeSession = stripeService.Get(orderHeader.SessionId);

            if (stripeSession.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(id, stripeSession.Id, stripeSession.PaymentIntentId);
                _unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(id, CONST_OrderStatus.Approved, CONST_PaymentStatus.Approved);
                _unitOfWork.Save();

                var shoppingCartsForRemove = _unitOfWork.ShoppingCartRepository
                    .GetAll(s => s.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCartRepository.DeleteRange(shoppingCartsForRemove);
                _unitOfWork.Save();
            }

            return View(id);
        }


        #endregion


        #region PRIVATE METHODS


        private static double CalculateOrderTotal(ShoppingCartVM shopCartVM)
        {
            foreach (var cart in shopCartVM.ShoppingCartItems)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shopCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return shopCartVM.OrderHeader.OrderTotal;
        }

        private static double GetPriceBasedOnQuantity(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem.Product is null) throw new ArgumentException("Invalid Product");

            if (shoppingCartItem.Count <= 50) return shoppingCartItem.Product.PriceUp50;
            else if (shoppingCartItem.Count <= 100) return shoppingCartItem.Product.PriceUp100;
            else if (shoppingCartItem.Count > 100) return shoppingCartItem.Product.PriceAbove100;
            else return 0;
        }

        private Session PaymentForStripe(ShoppingCartVM shopCartVM)
        {
            var host = $"{Request.Scheme}://{Request.Host.Value}";
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{host}/Customer/Cart/OrderConfirmation?id={shopCartVM.OrderHeader.Id}",
                CancelUrl = $"{host}/Customer/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            var sessionLineItems = shopCartVM.ShoppingCartItems
                .Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "brl",
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = item.Product?.Title }
                    },
                    Quantity = item.Count
                }).ToList();
            options.LineItems.AddRange(sessionLineItems);

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(
                shopCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            return session;
        }


        #endregion
    }


}
