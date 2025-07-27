using FootballManager.Data;
using FootballManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FootballManager.Web.Controllers
{
    public class TeamsController : Controller
    {
        private readonly FootballManagerDbContext _context;

        public TeamsController(FootballManagerDbContext context)
        {
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var teams = await _context.Teams.ToListAsync();
            return View(teams);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams
                .Include(t => t.Players)
                .Include(t => t.StaffMembers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (team == null) return NotFound();

            // Pass all league names for correct league link mapping
            var allLeagueNames = await _context.Teams
                .Select(t => t.LeagueName)
                .Distinct()
                .Where(n => !string.IsNullOrEmpty(n))
                .ToListAsync();
            ViewBag.AllLeagueNames = allLeagueNames;

            return View(team);
        }
    }
}