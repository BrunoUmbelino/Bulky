using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.ViewModels
{
    public class UserRoleVM
    {
        [Required] public string Id { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = new ApplicationUser();
        public IEnumerable<SelectListItem> RoleListItems { get; set; } = null!;
        public IEnumerable<SelectListItem> CompanyListItems { get; set; } = null!;
    }
}
