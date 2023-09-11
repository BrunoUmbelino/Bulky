using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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
            //if (!orderDetails.Any()) return NotFound();

            OrderVM orderVM = new()
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            return View(orderVM);
        }

        #region API CALLS

        [HttpGet]
        [Route("API/[area]/[controller]/GetAll")]
        public IActionResult GetAll(string? filterStatus = "all")
        {
            var orderHeaders = _unitOfWork.OrderHeaderRepository
                .GetAll(includeProperties: "ApplicationUser").ToList();

            var filteredOrderHeaders = filterStatus switch
            {
                "all" => orderHeaders,
                "inProcess" => orderHeaders.Where(o=>o.OrderStatus == Constants_OrderStatus.InProcess),
                "pending" => orderHeaders.Where(o=>o.PaymentStatus == Constants_OrderStatus.Pending),
                "approved" => orderHeaders.Where(o=>o.OrderStatus == Constants_OrderStatus.Approved),
                "completed" => orderHeaders.Where(o=>o.OrderStatus == Constants_OrderStatus.Shipped),
                _ => orderHeaders
            };

            return Json(new { success = true, data = filteredOrderHeaders });
        }

        #endregion
    }
}
