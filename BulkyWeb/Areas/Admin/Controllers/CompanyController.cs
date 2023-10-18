using AutoMapper;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CONST_Roles.Admin)]
    public class CompanyController : Controller
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CompanyController(IUnitOfWork unitOfWork, ILogger<CompanyController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region ACTIONS

        public ActionResult Index()
        {
            var companiesVM = _unitOfWork.CompanyRepo.GetAll().Select(c=>_mapper.Map<CategoryVM>(c));
            return View(companiesVM);
        }

        public ActionResult Upsert(int id)
        {
            if (id == 0) return NotFound();
            var company = _unitOfWork.CompanyRepo.Get(c => c.Id == id);
            if (company == null) return NotFound();

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(Company company)
        {
            try
            {
                if (!ModelState.IsValid) return View(company);

                string? messageAction = null;
                if (company.Id == 0)
                {
                    messageAction = "create";
                    _unitOfWork.CompanyRepo.Add(company);
                }
                else
                {
                    messageAction = "update";
                    _unitOfWork.CompanyRepo.update(company);
                }

                _unitOfWork.Save();
                TempData["successMessage"] = $"Company {company.Name} {messageAction} successfully";
                return RedirectToAction("Index");
            }
            catch
            {
                return View(company);
            }
        }

        // GET: CompanyController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            var company = _unitOfWork.CompanyRepo.Get(c => c.Id == id);
            if (company == null) return NotFound();

            return View(company);
        }

        #endregion

        #region API CALLS

        [HttpGet]
        [Route("API/[area]/[controller]/GetAll")]
        public ActionResult GetAllApi()
        {
            var companies = _unitOfWork.CompanyRepo.GetAll();
            return Json(new { success = true, data = companies });
        }

        [HttpDelete]
        [Route("API/[area]/[controller]/Delete")]
        public IActionResult DeleteApi(int id)
        {
            if (id == 0) return Json(new { success = false, message = "Invalid Id" });
            var company = _unitOfWork.CompanyRepo.Get(c => c.Id == id);
            if (company == null) return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.CompanyRepo.Delete(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}
