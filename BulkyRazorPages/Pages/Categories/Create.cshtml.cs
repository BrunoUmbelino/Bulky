using BulkyRazorPages.Data;
using BulkyRazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyRazorPages.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        
        public Category category;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            
        }

        public IActionResult OnPost(Category category)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Categories.Add(category);
            _context.SaveChanges();
            TempData["successMessage"] = "A Categoria foi criada com sucesso.";
            return RedirectToPage("Index");
        }
    }
}
