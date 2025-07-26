using FootballManager.Data;
using FootballManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FootballManager.Web.Controllers
{
    public class PlayersController : Controller
    {
        private readonly FootballManagerDbContext _context;

        public PlayersController(FootballManagerDbContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            var players = await _context.Players.Include(p => p.Team).ToListAsync();
            return View(players);
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (player == null) return NotFound();

            return View(player);
        }
    }
}