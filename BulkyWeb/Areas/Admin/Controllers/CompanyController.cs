using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }

        #region ACTIONS

        // GET: CompanyController
        public ActionResult Index()
        {
            var companies = _unitOfWork.CompanyRepository.GetAll();
            return View(companies);
        }

        // GET: CompanyController/Upsert
        public ActionResult Upsert(int id)
        {
            if (id == 0) return NotFound();
            var company = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
            if (company == null) return NotFound();

            return View(company);
        }

        // POST: CompanyController/Upsert
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
                    _unitOfWork.CompanyRepository.Add(company);
                }
                else
                {
                    messageAction = "update";
                    _unitOfWork.CompanyRepository.update(company);
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
            var company = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
            if (company == null) return NotFound();

            return View(company);
        }

        #endregion

        #region API CALLS

        [HttpGet]
        [Route("API/[area]/[controller]/GetAll")]
        public ActionResult GetAllApi()
        {
            var companies = _unitOfWork.CompanyRepository.GetAll();
            return Json(new { success = true, data = companies });
        }

        [HttpDelete]
        [Route("API/[area]/[controller]/Delete")]
        public IActionResult DeleteApi(int id)
        {
            if (id == 0) return Json(new { success = false, message = "Invalid Id" });
            var company = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
            if (company == null) return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.CompanyRepository.Delete(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}
