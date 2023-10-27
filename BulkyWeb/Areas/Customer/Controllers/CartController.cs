using AutoMapper;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Stripe.Checkout;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartController> _logger;
        private readonly IMapper _mapper;
        private string? _userId => (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [BindProperty]
        public ShopCartVM ShoppingCartVM { get; set; } = null!;

        public CartController(IUnitOfWork unitOfWork, ILogger<CartController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region ACTIONS

        public IActionResult Index()
        {
            if (_userId == null) throw new Exception("User not found");

            var shopCart = _unitOfWork.ShopCartRepo.GetFunc(
                filter: s => s.ApplicationUserId == _userId,
                include: query => query.Include(i => i.ShopCartItems).ThenInclude(t => t.Product));
            if (shopCart == null)
                return View(new ShopCartVM());

            var productImages = _unitOfWork.ProductImageRepo.GetAll();
            foreach (var item in shopCart.ShopCartItems)
            {
                var image = productImages.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
                if (image != null) item.Product.Images.Add(image);
            }

            return View(_mapper.Map<ShopCartVM>(shopCart));
        }

        public IActionResult PlusCartItem(int cartItemId)
        {
            try
            {
                if (cartItemId == 0)
                    return ResourceNotFound();

                var shopCart = _unitOfWork.ShopCartRepo.GetFunc(
                    filter: s => s.ApplicationUserId == _userId,
                    include: query => query.Include(i => i.ShopCartItems).ThenInclude(t => t.Product));
                if (shopCart == null)
                    return ResourceNotFound();

                var shopItem = shopCart.ShopCartItems.Where(s => s.Id == cartItemId).FirstOrDefault();
                if (shopItem == null)
                    return ResourceNotFound();

                shopItem.Quantity += 1;
                shopItem.SetUnitPrice(shopItem.Product);
                shopItem.CalculateTotalPrice();
                shopItem.UpdatedOn = DateTime.Now;

                shopCart.CalculateTotalValue();
                shopCart.UpdatedOn = DateTime.Now;

                _unitOfWork.ShopCartRepo.Update(shopCart);
                _unitOfWork.Save();

                TempData["successMessage"] = "Cart updated sucessfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Erro no processo de adicionar uma quantidade de item ao carrinho.");
                TempData["errorMessage"] = $"Something went wrong but don't be sad, it wasn't you fault.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult MinusCartItem(int cartItemId)
        {
            try
            {
                if (cartItemId == 0) return ResourceNotFound();

                var shopCart = _unitOfWork.ShopCartRepo.GetFunc(
                    filter: s => s.ApplicationUserId == _userId,
                    include: query => query.Include(i => i.ShopCartItems).ThenInclude(t => t.Product));
                if (shopCart == null) return ResourceNotFound();

                var shopItem = shopCart.ShopCartItems.Where(s => s.Id == cartItemId).FirstOrDefault();
                if (shopItem == null) return ResourceNotFound();

                if (shopItem.Quantity <= 1) RedirectToAction(nameof(Index));

                shopItem.Quantity -= 1;
                shopItem.SetUnitPrice(shopItem.Product);
                shopItem.CalculateTotalPrice();
                shopItem.UpdatedOn = DateTime.Now;

                shopCart.CalculateTotalValue();
                shopCart.UpdatedOn = DateTime.Now;

                _unitOfWork.ShopCartRepo.Update(shopCart);
                _unitOfWork.Save();

                TempData["successMessage"] = "Cart updated sucessfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Erro no processo de remover uma quantidade de item ao carrinho.");
                TempData["errorMessage"] = $"Something went wrong but don't be sad, it wasn't you fault.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult RemoveCartItem(int cartItemId)
        {
            try
            {
                if (cartItemId == 0) return ResourceNotFound();

                var shopCart = _unitOfWork.ShopCartRepo.GetFunc(
                    filter: s => s.ApplicationUserId == _userId,
                    include: query => query.Include(i => i.ShopCartItems).ThenInclude(t => t.Product));
                if (shopCart == null) return ResourceNotFound();

                var shopItem = shopCart.ShopCartItems.Where(s => s.Id == cartItemId).FirstOrDefault();
                if (shopItem == null) return ResourceNotFound();

                shopCart.ShopCartItems.Remove(shopItem);
                shopCart.CalculateTotalValue();
                shopCart.UpdatedOn = DateTime.Now;

                _unitOfWork.ShopCartRepo.Update(shopCart);
                _unitOfWork.Save();

                TempData["successMessage"] = "Cart updated sucessfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Erro no processo de remover item do carrinho.");
                TempData["errorMessage"] = $"Something went wrong but don't be sad, it wasn't you fault.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Summary()
        {
            var shopCart = _unitOfWork.ShopCartRepo.GetFunc(
                filter: s => s.ApplicationUserId == _userId,
                include: query => query.Include(i => i.ShopCartItems).ThenInclude(t => t.Product));
            if (shopCart == null) return ResourceNotFound();

            ShopCartVM shopCartVM = _mapper.Map<ShopCartVM>(shopCart);
            shopCartVM.Delivery.DateDelivery = DateTime.Now.AddDays(14);
            return View(shopCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ShopCartVM shopCartVM)
        {
            try
            {
                var shopCart = _unitOfWork.ShopCartRepo.GetFunc(
                    filter: s => s.ApplicationUserId == _userId,
                    include: query => query.Include(i => i.ShopCartItems).ThenInclude(t => t.Product));
                if (shopCart == null || _userId == null)
                    return ResourceNotFound();

                var applicationUser = _unitOfWork.ApplicationUserRepo.Get(u => u.Id == _userId);
                if (applicationUser is null) return ResourceNotFound();

                Order order = new()
                {
                    ApplicationUserId = _userId,
                    Delivery = shopCartVM.Delivery,
                    Payment = new Payment() { },
                    PurchaseItems = shopCart.ShopCartItems.Select(s => _mapper.Map<PurchaseItem>(s)).ToList(),
                    AddedIn = DateTime.Now,
                    TotalValue = shopCart.TotalValue

                };

                bool isCompanyAccout = applicationUser.CompanyId.GetValueOrDefault() != 0;
                if (isCompanyAccout)
                {
                    order.Status = CONST_OrderStatus.Approved;
                    order.Payment.PaymentStatus = CONST_PaymentStatus.DelayedPayment;

                    _unitOfWork.OrderRepo.Update(order);
                    _unitOfWork.Save();
                }
                else
                {
                    order.Status = CONST_OrderStatus.Pending;
                    order.Payment.PaymentStatus = CONST_PaymentStatus.Pending;
                    _unitOfWork.OrderRepo.Update(order);

                    var stripeSession = StripePayment(order);
                    _unitOfWork.Save();

                    Response.Headers.Location = stripeSession.Url;
                    return new StatusCodeResult(303);
                }

                return RedirectToAction(nameof(OrderConfirmation), new { id = order.Id });

                //string? userId = (User.Identity as ClaimsIdentity)?
                //    .FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //if (userId == null) return NotFound();

                //shopCartVM.ShoppingCartItems = _unitOfWork.ShopCartItemRepo
                //    .GetAll(filter: sc => sc.ApplicationUserId == userId, $"{nameof(ShopCart.Product)}");
                //shopCartVM.OrderHeader.ApplicationUserId = userId;
                //shopCartVM.OrderHeader.OrderDate = DateTime.Now;
                //shopCartVM.OrderHeader.OrderTotal = CalculateOrderTotal(shopCartVM);

                //var applicationUser = _unitOfWork.ApplicationUserRepo.Get(u => u.Id == userId);
                //if (applicationUser is null) return NotFound();

                //bool isCompanyAccout = applicationUser.CompanyId.GetValueOrDefault() != 0;
                //bool isCustomerAccout = !isCompanyAccout;
                //if (isCustomerAccout)
                //{
                //    shopCartVM.OrderHeader.PaymentStatus = CONST_PaymentStatus.Pending;
                //    shopCartVM.OrderHeader.OrderStatus = CONST_OrderStatus.Pending;
                //}
                //if (isCompanyAccout)
                //{
                //    shopCartVM.OrderHeader.PaymentStatus = CONST_PaymentStatus.DelayedPayment;
                //    shopCartVM.OrderHeader.OrderStatus = CONST_PaymentStatus.Approved;
                //}
                //_unitOfWork.OrderHeaderRepo.Add(shopCartVM.OrderHeader);
                //_unitOfWork.Save();

                //foreach (var item in shopCartVM.ShoppingCartItems)
                //{
                //    OrderDetail orderDetail = new()
                //    {
                //        ProductId = item.ProductId,
                //        OrderHeaderId = shopCartVM.OrderHeader.Id,
                //        Price = item.Price,
                //        Count = item.Count,
                //    };
                //    _unitOfWork.OrderDetailRepo.Add(orderDetail);
                //    _unitOfWork.Save();
                //}

                //if (isCustomerAccout)
                //{
                //    var session = PaymentForStripe(shopCartVM);
                //    Response.Headers.Location = session.Url;
                //    return new StatusCodeResult(303);
                //}

                //return RedirectToAction(nameof(OrderConfirmation), new { id = shopCartVM.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Erro na criação de Ordem.");
                TempData["errorMessage"] = $"{ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        public IActionResult OrderConfirmation(int id)
        {
            return View();
            //OrderHeader orderHeader = _unitOfWork.OrderHeaderRepo.Get(o => o.Id == id);
            //if (orderHeader == null) return NotFound();

            //var isOrderByCompany = orderHeader.PaymentStatus == CONST_PaymentStatus.DelayedPayment;
            //if (isOrderByCompany)
            //{
            //    var shoppingCartsForRemove = _unitOfWork.ShopCartItemRepo
            //        .GetAll(s => s.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            //    _unitOfWork.ShopCartItemRepo.DeleteRange(shoppingCartsForRemove);
            //    _unitOfWork.Save();

            //    return View(id);
            //}

            //var stripeService = new SessionService();
            //Session stripeSession = stripeService.Get(orderHeader.SessionId);

            //if (stripeSession.PaymentStatus.ToLower() == "paid")
            //{
            //    _unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(id, stripeSession.Id, stripeSession.PaymentIntentId);
            //    _unitOfWork.OrderHeaderRepo.UpdateOrderPaymentStatus(id, CONST_OrderStatus.Approved, CONST_PaymentStatus.Approved);
            //    _unitOfWork.Save();

            //    var shoppingCartsForRemove = _unitOfWork.ShopCartItemRepo
            //        .GetAll(s => s.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            //    _unitOfWork.ShopCartItemRepo.DeleteRange(shoppingCartsForRemove);
            //    _unitOfWork.Save();
            //}

            //return View(id);
        }


        #endregion


        #region PRIVATE METHODS


        IActionResult ResourceNotFound()
        {
            TempData["errorMessage"] = "Resource not found";
            return RedirectToAction(nameof(Index));
        }

        private Session StripePayment(Order order)
        {
            var host = $"{Request.Scheme}://{Request.Host.Value}";
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{host}/Customer/Cart/OrderConfirmation?id={order.Id}",
                CancelUrl = $"{host}/Customer/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            var sessionLineItems = order.PurchaseItems
                .Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.UnitPrice * 100),
                        Currency = "brl",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product?.Title
                        }
                    },
                    Quantity = item.Quantity
                })
                .ToList();
            options.LineItems.AddRange(sessionLineItems);

            var service = new SessionService();
            Session session = service.Create(options);
            order.Payment.SessionId = session.Id;
            order.Payment.PaymentIntentId = session.PaymentIntentId;

            _unitOfWork.OrderRepo.Update(order);
            return session;
        }


        #endregion
    }
}
