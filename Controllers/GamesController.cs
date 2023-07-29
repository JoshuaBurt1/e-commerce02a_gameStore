using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mage.Data;
using Mage.Models;
using Microsoft.AspNetCore.Authorization;

namespace Mage.Controllers
{
    //Only administrator is allowed to access page
    [Authorize(Roles = "Administrator")]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            //SQL: Games = TABLE; include = JOIN; ORDER BY game name alphabetically
            var applicationDbContext = _context.Games.Include(g => g.Category).OrderBy(g => g.Name);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            //if no categories, user cannot create a product
            if(!_context.Categories.Any())
            {
                ModelState.AddModelError("", "No categories exist. Please create a category first.");
                return RedirectToAction("Index", "Departments");
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["GameSizeUnit"] = new SelectList(Enum.GetValues(typeof(GameSizeUnit))); //needed to get GameSizeUnit value
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Name,Description,Genre,Price,Size,SizeUnit")] Game game, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                game.Photo = await UploadPhoto(Photo); 
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
            ViewData["GameSizeUnit"] = new SelectList(Enum.GetValues(typeof(GameSizeUnit))); //needed to get GameSizeUnit value
            return View(game);
        }

        private async Task<string> UploadPhoto(IFormFile photo)
        {
            if (photo != null)
            {
                //get temp location
                var filePath = Path.GetTempFileName();
                // create unique name so we don't overwrite an existing photo
                var fileName = Guid.NewGuid() + "-" + photo.FileName;
                //set the destination dynamically
                var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\images\\games\\" + fileName;
                //Execute the file copy
                using var stream = new FileStream(uploadPath, FileMode.Create);
                await photo.CopyToAsync(stream);
                return fileName;
            }
            return null;
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
            ViewData["GameSizeUnit"] = new SelectList(Enum.GetValues(typeof(GameSizeUnit))); //needed to get GameSizeUnit value
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Name,Description,Genre,Price,Size,SizeUnit,Photo")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
            ViewData["GameSizeUnit"] = new SelectList(Enum.GetValues(typeof(GameSizeUnit))); //needed to get GameSizeUnit value
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Games'  is null.");
            }
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
          return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
