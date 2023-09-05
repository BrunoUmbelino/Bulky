using Bulky.DataAccess.Repository.IRepository;
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

        #region API CALLS

        [HttpGet]
        [Route("API/[area]/[controller]/GetAll")]
        public IActionResult GetAll()
        {
            var orderHeaders = _unitOfWork.OrderHeaderRepository
                .GetAll(includeProperties: "ApplicationUser").ToList();
            return Json(new { success = true, data = orderHeaders });
        }

        #endregion
    }
}
