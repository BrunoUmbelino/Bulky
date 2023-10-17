using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartController> _logger;

        [BindProperty]
        public ShopCartVM ShoppingCartVM { get; set; } = null!;

        public CartController(IUnitOfWork unitOfWork, ILogger<CartController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region ACTIONS


        //public IActionResult Index()
        //{
        //    string? userId = (User.Identity as ClaimsIdentity)?
        //        .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    ShoppingCartVM shopCartVM = new()
        //    {
        //        ShoppingCartItems = _unitOfWork.ShopCartItemRepo.GetAll(
        //            filter: sc => sc.ApplicationUserId == userId,
        //            includeProperties: $"{nameof(ShopCart.Product)}"),
        //        OrderHeader = new()
        //    };

        //    var productImages = _unitOfWork.ProductImageRepo.GetAll();
        //    foreach (var cart in shopCartVM.ShoppingCartItems)
        //    {
        //        if (cart.Product != null) 
        //            cart.Product.Images = productImages.Where(i => i.ProductId == cart.ProductId).ToList();
        //        cart.Price = GetPriceBasedOnQuantity(cart);
        //        shopCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        //    }

        //    return View(shopCartVM);
        //}

        //public IActionResult PlusCartItem(int cartItemId)
        //{
        //    if (cartItemId == 0) return NotFound();
        //    var cartItemDB = _unitOfWork.ShopCartItemRepo.Get(sc => sc.Id == cartItemId, includeProperties: $"{nameof(ShopCart.Product)}");
        //    if (cartItemDB == null) return NotFound();

        //    cartItemDB.Count += 1;
        //    cartItemDB.Price = GetPriceBasedOnQuantity(cartItemDB);

        //    _unitOfWork.ShopCartItemRepo.Update(cartItemDB);
        //    _unitOfWork.Save();

        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult MinusCartItem(int cartItemId)
        //{
        //    if (cartItemId == 0) return NotFound();
        //    var cartItemDB = _unitOfWork.ShopCartItemRepo.Get(sc => sc.Id == cartItemId, $"{nameof(ShopCart.Product)}");
        //    if (cartItemDB == null) return NotFound();

        //    cartItemDB.Count -= 1;
        //    if (cartItemDB.Count <= 0)
        //    {
        //        _unitOfWork.ShopCartItemRepo.Delete(cartItemDB);
        //        _unitOfWork.Save();

        //        var cartItemsQuantity = _unitOfWork.ShopCartItemRepo.GetAll(s => s.ApplicationUserId == cartItemDB.ApplicationUserId)?.Count();
        //        if (cartItemsQuantity is not null)
        //            HttpContext.Session.SetInt32(CONST_Session.ShoppingCart, (int)cartItemsQuantity);
        //    }
        //    else
        //    {
        //        cartItemDB.Price = GetPriceBasedOnQuantity(cartItemDB);
        //        _unitOfWork.ShopCartItemRepo.Update(cartItemDB);
        //        _unitOfWork.Save();
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult RemoveCartItem(int cartItemId)
        //{
        //    if (cartItemId == 0) return NotFound();
        //    var cartItemDB = _unitOfWork.ShopCartItemRepo.Get(sc => sc.Id == cartItemId, $"{nameof(ShopCart.Product)}");
        //    if (cartItemDB == null) return NotFound();

        //    _unitOfWork.ShopCartItemRepo.Delete(cartItemDB);
        //    _unitOfWork.Save();

        //    var cartItemsQuantity = _unitOfWork.ShopCartItemRepo.GetAll(s => s.ApplicationUserId == cartItemDB.ApplicationUserId)?.Count();
        //    if (cartItemsQuantity is not null)
        //        HttpContext.Session.SetInt32(CONST_Session.ShoppingCart, (int)cartItemsQuantity);

        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult Summary()
        //{
        //    string? userId = (User.Identity as ClaimsIdentity)?
        //        .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (userId == null) return NotFound();

        //    var userFromDb = _unitOfWork.ApplicationUserRepo.Get(u => u.Id == userId);
        //    if (userFromDb == null) return NotFound();

        //    ShoppingCartVM shopCartVM = new()
        //    {
        //        ShoppingCartItems = _unitOfWork.ShopCartItemRepo
        //            .GetAll(filter: sc => sc.ApplicationUserId == userId, $"{nameof(ShopCart.Product)}"),
        //        OrderHeader = new()
        //        {
        //            ApplicationUser = userFromDb,
        //            PhoneNumber = userFromDb.PhoneNumber ?? string.Empty,
        //            StreetAddress = userFromDb.StreetAddress ?? string.Empty,
        //            City = userFromDb.City ?? string.Empty,
        //            State = userFromDb.State ?? string.Empty,
        //            PostalCode = userFromDb.PostalCode ?? string.Empty,
        //            Name = userFromDb.Name ?? string.Empty,
        //        }
        //    };
        //    shopCartVM.OrderHeader.OrderTotal = CalculateOrderTotal(shopCartVM);

        //    return View(shopCartVM);
        //}

        //[HttpPost]
        //[ActionName("Summary")]
        //public IActionResult SummaryPost(ShoppingCartVM shopCartVM)
        //{
        //    try
        //    {
        //        string? userId = (User.Identity as ClaimsIdentity)?
        //            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        if (userId == null) return NotFound();

        //        shopCartVM.ShoppingCartItems = _unitOfWork.ShopCartItemRepo
        //            .GetAll(filter: sc => sc.ApplicationUserId == userId, $"{nameof(ShopCart.Product)}");
        //        shopCartVM.OrderHeader.ApplicationUserId = userId;
        //        shopCartVM.OrderHeader.OrderDate = DateTime.Now;
        //        shopCartVM.OrderHeader.OrderTotal = CalculateOrderTotal(shopCartVM);

        //        var applicationUser = _unitOfWork.ApplicationUserRepo.Get(u => u.Id == userId);
        //        if (applicationUser is null) return NotFound();

        //        bool isCompanyAccout = applicationUser.CompanyId.GetValueOrDefault() != 0;
        //        bool isCustomerAccout = !isCompanyAccout;
        //        if (isCustomerAccout)
        //        {
        //            shopCartVM.OrderHeader.PaymentStatus = CONST_PaymentStatus.Pending;
        //            shopCartVM.OrderHeader.OrderStatus = CONST_OrderStatus.Pending;
        //        }
        //        if (isCompanyAccout)
        //        {
        //            shopCartVM.OrderHeader.PaymentStatus = CONST_PaymentStatus.DelayedPayment;
        //            shopCartVM.OrderHeader.OrderStatus = CONST_PaymentStatus.Approved;
        //        }
        //        _unitOfWork.OrderHeaderRepo.Add(shopCartVM.OrderHeader);
        //        _unitOfWork.Save();

        //        foreach (var item in shopCartVM.ShoppingCartItems)
        //        {
        //            OrderDetail orderDetail = new()
        //            {
        //                ProductId = item.ProductId,
        //                OrderHeaderId = shopCartVM.OrderHeader.Id,
        //                Price = item.Price,
        //                Count = item.Count,
        //            };
        //            _unitOfWork.OrderDetailRepo.Add(orderDetail);
        //            _unitOfWork.Save();
        //        }

        //        if (isCustomerAccout)
        //        {
        //            var session = PaymentForStripe(shopCartVM);
        //            Response.Headers.Location = session.Url;
        //            return new StatusCodeResult(303);
        //        }

        //        return RedirectToAction(nameof(OrderConfirmation), new { id = shopCartVM.OrderHeader.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(0, ex, "Erro na criação de Ordem.");
        //        TempData["errorMessage"] = $"{ex.Message}";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        //public IActionResult OrderConfirmation(int id)
        //{
        //    OrderHeader orderHeader = _unitOfWork.OrderHeaderRepo.Get(o => o.Id == id);
        //    if (orderHeader == null) return NotFound();

        //    var isOrderByCompany = orderHeader.PaymentStatus == CONST_PaymentStatus.DelayedPayment;
        //    if (isOrderByCompany)
        //    {
        //        var shoppingCartsForRemove = _unitOfWork.ShopCartItemRepo
        //            .GetAll(s => s.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
        //        _unitOfWork.ShopCartItemRepo.DeleteRange(shoppingCartsForRemove);
        //        _unitOfWork.Save();

        //        return View(id);
        //    }

        //    var stripeService = new SessionService();
        //    Session stripeSession = stripeService.Get(orderHeader.SessionId);

        //    if (stripeSession.PaymentStatus.ToLower() == "paid")
        //    {
        //        _unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(id, stripeSession.Id, stripeSession.PaymentIntentId);
        //        _unitOfWork.OrderHeaderRepo.UpdateOrderPaymentStatus(id, CONST_OrderStatus.Approved, CONST_PaymentStatus.Approved);
        //        _unitOfWork.Save();

        //        var shoppingCartsForRemove = _unitOfWork.ShopCartItemRepo
        //            .GetAll(s => s.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
        //        _unitOfWork.ShopCartItemRepo.DeleteRange(shoppingCartsForRemove);
        //        _unitOfWork.Save();
        //    }

        //    return View(id);
        //}


        #endregion


        #region PRIVATE METHODS


        //private static double CalculateOrderTotal(ShoppingCartVM shopCartVM)
        //{
        //    foreach (var cart in shopCartVM.ShoppingCartItems)
        //    {
        //        cart.Price = GetPriceBasedOnQuantity(cart);
        //        shopCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        //    }

        //    return shopCartVM.OrderHeader.OrderTotal;
        //}

        //private static double GetPriceBasedOnQuantity(ShopCart shoppingCartItem)
        //{
        //    if (shoppingCartItem.Product is null) throw new ArgumentException("Invalid Product");

        //    if (shoppingCartItem.Count <= 50) return shoppingCartItem.Product.PriceUp50;
        //    else if (shoppingCartItem.Count <= 100) return shoppingCartItem.Product.PriceUp100;
        //    else if (shoppingCartItem.Count > 100) return shoppingCartItem.Product.PriceAbove100;
        //    else return 0;
        //}

        //private Session PaymentForStripe(ShoppingCartVM shopCartVM)
        //{
        //    var host = $"{Request.Scheme}://{Request.Host.Value}";
        //    var options = new SessionCreateOptions
        //    {
        //        SuccessUrl = $"{host}/Customer/Cart/OrderConfirmation?id={shopCartVM.OrderHeader.Id}",
        //        CancelUrl = $"{host}/Customer/Cart/Index",
        //        LineItems = new List<SessionLineItemOptions>(),
        //        Mode = "payment",
        //    };

        //    var sessionLineItems = shopCartVM.ShoppingCartItems
        //        .Select(item => new SessionLineItemOptions
        //        {
        //            PriceData = new SessionLineItemPriceDataOptions
        //            {
        //                UnitAmount = (long)(item.Price * 100),
        //                Currency = "brl",
        //                ProductData = new SessionLineItemPriceDataProductDataOptions { Name = item.Product?.Title }
        //            },
        //            Quantity = item.Count
        //        }).ToList();
        //    options.LineItems.AddRange(sessionLineItems);

        //    var service = new SessionService();
        //    Session session = service.Create(options);
        //    _unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(
        //        shopCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
        //    _unitOfWork.Save();
        //    return session;
        //}


        #endregion
    }


}
