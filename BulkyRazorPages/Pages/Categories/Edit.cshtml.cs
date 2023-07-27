using BulkyRazorPages.Data;
using BulkyRazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyRazorPages.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public Category category { get; set; }

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet(int? id)
        {
            if (id != null || id != 0)
                category = _context.Categories.FirstOrDefault(c => c.Id == id) ?? category;
        }

        public IActionResult OnPost(Category category)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Categories.Update(category);
            _context.SaveChanges();
            TempData["successMessage"] = "A Categoria foi alterada com sucesso.";
            return RedirectToPage("Index");
        }
    }
}
