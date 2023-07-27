using BulkyRazorPages.Data;
using BulkyRazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyRazorPages.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        public List<Category> categories;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            categories = _context.Categories.ToList();
        }
    }
}
