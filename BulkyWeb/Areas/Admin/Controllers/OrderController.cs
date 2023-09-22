using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IUnitOfWork unitOfWork, ILogger<OrderController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region ACTIONS


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            if (orderId == 0) return NotFound();
            var orderHeader = _unitOfWork.OrderHeaderRepository.Get(oh => oh.Id == orderId, includeProperties: "ApplicationUser");
            if (orderHeader == null) return NotFound();
            var orderDetails = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product");
            if (!orderDetails.Any()) return NotFound();

            OrderVM orderVM = new()
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles = $"{CONST_Roles.Admin},{CONST_Roles.Employee}")]
        public IActionResult UpdateOrderDetails(OrderVM orderVM)
        {
            try
            {
                var orderHeaderDB = _unitOfWork.OrderHeaderRepository.Get(o => o.Id == orderVM.OrderHeader.Id);
                if (orderHeaderDB == null) return NotFound();

                orderHeaderDB.Name = orderVM.OrderHeader.Name;
                orderHeaderDB.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
                orderHeaderDB.StreetAddress = orderVM.OrderHeader.StreetAddress;
                orderHeaderDB.City = orderVM.OrderHeader.City;
                orderHeaderDB.State = orderVM.OrderHeader.State;
                orderHeaderDB.PostalCode = orderVM.OrderHeader.PostalCode;
                if (!String.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
                    orderHeaderDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                if (!String.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
                    orderHeaderDB.Carrier = orderVM.OrderHeader.Carrier;

                _unitOfWork.OrderHeaderRepository.Update(orderHeaderDB);
                _unitOfWork.Save();

                TempData["successMessage"] = "Order Detail updated successfully";
                return RedirectToAction(nameof(Details), new { orderId = orderHeaderDB.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Erro em UpdateOrderDetails");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = $"{CONST_Roles.Admin},{CONST_Roles.Employee}")]
        public IActionResult StartProcessing(int orderId)
        {
            try
            {
                _unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(orderId, CONST_OrderStatus.InProcess);
                _unitOfWork.Save();

                TempData["successMessage"] = "Order Detail is in process";
                return RedirectToAction(nameof(Details), new { orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(null, ex, "Error in StartProcessing");
                TempData["errorMessage"] = "Something went wrong but don't be sad, it wasn't your fault.";
                return RedirectToAction(nameof(Details), new { orderId });
            }

        }

        [HttpPost]
        [Authorize(Roles = $"{CONST_Roles.Admin},{CONST_Roles.Employee}")]
        public IActionResult ShipOrder(OrderVM orderVM)
        {
            try
            {
                var orderHeaderDB = _unitOfWork.OrderHeaderRepository.Get(o => o.Id == orderVM.OrderHeader.Id);
                if (orderHeaderDB == null) return NotFound();

                orderHeaderDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                orderHeaderDB.Carrier = orderVM.OrderHeader.Carrier;
                orderHeaderDB.OrderStatus = CONST_OrderStatus.Shipped;
                orderHeaderDB.ShippingDate = DateTime.Now;
                if (orderHeaderDB.PaymentStatus == CONST_PaymentStatus.DelayedPayment)
                    orderHeaderDB.PaymentDueDate = DateTime.Now.AddDays(30);

                _unitOfWork.OrderHeaderRepository.Update(orderHeaderDB);
                _unitOfWork.Save();

                TempData["successMessage"] = "Order Shipped Successfuly";
                return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error in ShipOrder");
                TempData["errorMessage"] = "Something went wrong but don't be sad, it wasn't your fault.";
                return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
            }
        }

        [HttpPost]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            try
            {
                var orderHeaderDB = _unitOfWork.OrderHeaderRepository.Get(o => o.Id == orderVM.OrderHeader.Id);
                if (orderHeaderDB == null) return NotFound();

                if (orderHeaderDB.PaymentStatus == CONST_PaymentStatus.Approved)
                {
                    var refundOptions = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeaderDB.PaymentIntentId
                    };

                    var stripeRefundService = new RefundService();
                    Refund refund = stripeRefundService.Create((refundOptions));

                    _unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(
                        orderHeaderDB.Id, CONST_OrderStatus.Cancelled, CONST_PaymentStatus.Refunded);
                }
                else
                {
                    _unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(
                        orderHeaderDB.Id, CONST_OrderStatus.Cancelled, CONST_PaymentStatus.Cancelled);
                }
                _unitOfWork.Save();

                TempData["successMessage"] = "Order cancelled successfully";
                return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error in CancelOrder");
                TempData["errorMessage"] = "Something went wrong but don't be sad, it wasn't your fault.";
                return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
            }
        }

        [HttpPost]
        public IActionResult PayNowForCompanyOrder(OrderVM orderVM)
        {
            try
            {
                var orderHeaderDB = _unitOfWork.OrderHeaderRepository.Get(o => o.Id == orderVM.OrderHeader.Id);
                if (orderHeaderDB == null) return NotFound();
                var orderDetailsDB = _unitOfWork.OrderDetailRepository.GetAll(o => o.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: "Product");
                if (orderDetailsDB == null) return NotFound();

                var session = PaymentForStripe(new OrderVM { OrderHeader = orderHeaderDB, OrderDetails = orderDetailsDB });
                Response.Headers.Location = session.Url;
                return new StatusCodeResult(303);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error in PayNowForCompanyOrder");
                TempData["errorMessage"] = "Something went wrong but don't be sad, it wasn't your fault.";
                return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
            }
            
        }

        public IActionResult PaymentConfirmation(int orderId)
        {
            try
            {
                var orderHeaderDB = _unitOfWork.OrderHeaderRepository.Get(o => o.Id == orderId);
                if (orderHeaderDB == null) return NotFound();

                if (orderHeaderDB.PaymentStatus == CONST_PaymentStatus.DelayedPayment)
                {
                    var stripeService = new SessionService();
                    var stripeSession = stripeService.Get(orderHeaderDB.SessionId);

                    if (stripeSession.PaymentStatus.ToLower() == "paid")
                    {
                        _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(orderId, stripeSession.Id, stripeSession.PaymentIntentId);
                        _unitOfWork.OrderHeaderRepository.UpdateOrderPaymentStatus(orderId, paymentStatus: CONST_PaymentStatus.Approved);
                        _unitOfWork.Save();
                    };
                }

                return View(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error in PaymentConfirmation");
                TempData["errorMessage"] = "Something went wrong but don't be sad, it wasn't your fault.";
                return RedirectToAction(nameof(Details), new { orderId });
            }
            
        }


        #endregion

        #region API's
        [HttpGet]
        [Route("API/[area]/[controller]/GetAll")]
        public IActionResult GetAll(string? filterStatus = "all")
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(CONST_Roles.Admin) || User.IsInRole(CONST_Roles.Employee))
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();
            else
            {
                var userId = (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(o => o.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

            var filteredOrderHeaders = filterStatus switch
            {
                "all" => orderHeaders,
                "inProcess" => orderHeaders.Where(o => o.OrderStatus == CONST_OrderStatus.InProcess),
                "pending" => orderHeaders.Where(o => o.OrderStatus == CONST_OrderStatus.Pending),
                "approved" => orderHeaders.Where(o => o.OrderStatus == CONST_OrderStatus.Approved),
                "completed" => orderHeaders.Where(o => o.OrderStatus == CONST_OrderStatus.Shipped),
                _ => orderHeaders
            };

            return Json(new { success = true, data = filteredOrderHeaders });
        }
        #endregion

        #region PRIVATE METHODS


        private Session PaymentForStripe(OrderVM orderVM)
        {
            var host = $"{Request.Scheme}://{Request.Host.Value}";
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{host}/Admin/Order/PaymentConfirmation?orderId={orderVM.OrderHeader.Id}",
                CancelUrl = $"{host}/Admin/Order/details?orderId={orderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            var sessionLineItems = orderVM.OrderDetails
                .Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "brl",
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = item.Product?.Title }
                    },
                    Quantity = item.Count
                })
                .ToList();

            options.LineItems.AddRange(sessionLineItems);

            var stripeService = new SessionService();
            Session stripeSession = stripeService.Create(options);
            _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(
                orderVM.OrderHeader.Id, stripeSession.Id, stripeSession.PaymentIntentId);
            _unitOfWork.Save();

            return stripeSession;
        }


        #endregion
    }
}
