using ELibrary_Temp.Data;
using ELibrary_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ELibrary_Temp.Categories
{
    public class IndexModel : PageModel
    {
        public ApplicationDbContext _db;
        public List<Category> CategoryList { get; set; }
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        

        public void OnGet()
        {

            CategoryList = _db.Categories.ToList();

        }
    }
}
