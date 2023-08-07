using Microsoft.AspNetCore.Mvc;
using Mage.Data;
using Microsoft.EntityFrameworkCore;

namespace MajorGamer.Controllers
{
    public class ShopController : Controller
    {
        //Property for our database connection
        private ApplicationDbContext _context;

        //Constructor
        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.OrderBy(category => category.Name).ToListAsync();
            return View(categories);
        }
        public async Task<IActionResult> Details(int? id)
        {
            var categoryWithGames = await _context.Categories.Include(category => category.Games).FirstOrDefaultAsync(category => category.Id == id);
            return View(categoryWithGames);
        }
        public async Task<IActionResult> GameDetails(int? id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(game => game.Id == id);
            return View(game);
        }
    }
}
