using BulkyRazorPages.Data;
using BulkyRazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyRazorPages.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesDbContext _context;
        public List<Category> categories;

        public IndexModel(RazorPagesDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            categories = _context.Categories.ToList();
        }
    }
}
