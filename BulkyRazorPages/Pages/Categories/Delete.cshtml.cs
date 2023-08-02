using BulkyRazorPages.Data;
using BulkyRazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyRazorPages.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly RazorPagesDbContext _context;
        public Category category { get; set; }

        public DeleteModel(RazorPagesDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? id)
        {
            category = _context.Categories.FirstOrDefault(c => c.Id == id) ?? category;
            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (id == null || id == 0) return Page();
            category = _context.Categories.FirstOrDefault(c => c.Id == id) ?? category;
            if (category == null) return Page();

            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["successMessage"] = "A Categoria foi excluída com sucesso.";
            return RedirectToPage("Index");
        }
    }
}
