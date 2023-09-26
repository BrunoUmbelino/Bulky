using Bulky.DataAccess.Data;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = CONST_Roles.Admin)]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly AppDbContext _context;

        public UserController(AppDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserRoleManager(string userId)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var userVM = new UserRoleVM()
            {
                ApplicationUser = user,
                CompanyListItems = PopulateCompanyList(),
                RoleListItems = PopulateRoleList(),
            };
            string? userRoleId = _context.UserRoles.FirstOrDefault(r => r.UserId == user.Id)?.RoleId;
            string? roleName = _context.Roles.FirstOrDefault(r => r.Id == userRoleId)?.Name;
            if (roleName is not null) userVM.ApplicationUser.Role = roleName;

            return View(userVM);
        }

        [HttpPost]
        public IActionResult UserRoleManager(UserRoleVM userRoleVM)
        {
            try
            {
                //var userDb = _context.ApplicationUsers.FirstOrDefault(u=>u.Id == userRoleVM.ApplicationUser.Id);
                //if (userDb == null) return NotFound();

                //userDb.Role = userRoleVM.ApplicationUser.Role;
                //if (userRoleVM.ApplicationUser.Role == CONST_Roles.Company) userDb.CompanyId = userRoleVM.ApplicationUser.CompanyId;
                //else userDb.CompanyId = null;
                //_context.ApplicationUsers.Update(userDb);
                //_context.SaveChanges();

                var userRoleDb = _context.UserRoles.FirstOrDefault(r=>r.UserId == userRoleVM.Id);
                if (userRoleDb == null) return NotFound();

                userRoleDb.RoleId = userRoleVM.ApplicationUser.Role;
                _context.UserRoles.Update(userRoleDb);

                //if (userRoleVM.ApplicationUser.Role )

                _context.SaveChanges();

                TempData["successMessage"] = "User role updated successfuly.";
                userRoleVM.RoleListItems = PopulateRoleList();
                userRoleVM.CompanyListItems = PopulateCompanyList();
                return View(userRoleVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                TempData["errorMessage"] = "Operation update role failed";
                return RedirectToAction(nameof(Index));
            }
        }

        #region API 

        [HttpGet]
        [Route("API/[area]/[controller]/GetAll")]
        public IActionResult GetAll()
        {
            var users = _context.ApplicationUsers.Include(au => au.Company).ToList();
            if (users == null) return NotFound();

            var identityRoles = _context.Roles.ToList();
            if (identityRoles == null) return NotFound();

            var identityUsersAndRoles = _context.UserRoles.ToList();
            if (identityUsersAndRoles == null) return NotFound();

            foreach (var user in users)
            {
                var userRoleId = identityUsersAndRoles.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                user.Role = identityRoles.FirstOrDefault(u => u.Id == userRoleId)?.Name ?? "";
                user.Company ??= new Company() { Name = "" };
            }

            return Json(new { data = users });
        }

        [HttpPost]
        [Route("API/[area]/[controller]/LockUnlockAccount")]
        public IActionResult LockUnlockAccount([FromBody] string id)
        {
            try
            {
                var userDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
                if (userDb == null) 
                    return Json(new { success = false, message = "User not found" });

                var userIsLocked = userDb.LockoutEnd != null && userDb.LockoutEnd > DateTime.Now;
                if (userIsLocked) userDb.LockoutEnd = DateTime.Now;
                else userDb.LockoutEnd = DateTime.Now.AddYears(1000);
                _context.SaveChanges();

                var message = userIsLocked ? "Account UNLOCK successfully" : "Account LOCKED successfully";
                return Json(new { success = true, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return Json(new { success = false, message = "Operation Lock/Unlock Account failed" });
            }
        }

        #endregion

        private IEnumerable<SelectListItem> PopulateRoleList()
        {
            return _context.Roles.ToList().Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id,
            });
        }

        private IEnumerable<SelectListItem> PopulateCompanyList()
        {
            return _context.Companies.ToList().Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            }) ;
        }
    }
}
