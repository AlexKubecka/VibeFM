using FootballManager.Data;
using FootballManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FootballManager.Web.Controllers
{
    public class StaffMembersController : Controller
    {
        private readonly FootballManagerDbContext _context;

        public StaffMembersController(FootballManagerDbContext context)
        {
            _context = context;
        }

        // GET: StaffMembers
        public async Task<IActionResult> Index()
        {
            var staff = await _context.StaffMembers.Include(s => s.Team).ToListAsync();
            return View(staff);
        }

        // GET: StaffMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var staffMember = await _context.StaffMembers
                .Include(s => s.Team)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (staffMember == null) return NotFound();

            return View(staffMember);
        }
    }
}