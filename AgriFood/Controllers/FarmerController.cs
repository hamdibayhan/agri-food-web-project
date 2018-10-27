using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AgriFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgriFood.Controllers
{
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FarmerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Details(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Farmer = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            ViewBag.IsProductOwn = IsProductOwn(id);
            var Products = _context.Products.Include(p => p.Farmer)
                                            .Where(p => p.FarmerId == id & p.IsActive)
                                            .OrderByDescending(p => p.CreatedDate);
            return View(await Products.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Boolean IsProductOwn(string id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            return userId == id ? true : false;
        }
    }
}
