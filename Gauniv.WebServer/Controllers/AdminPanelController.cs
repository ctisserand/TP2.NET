using Gauniv.WebServer.Data;
using Gauniv.WebServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Gauniv.WebServer.Controllers
{
    public class AdminPanelController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, UserManager<User> userManager) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly ApplicationDbContext applicationDbContext = applicationDbContext;
        private readonly UserManager<User> userManager = userManager;


        public async Task<IActionResult> AdminIndex(string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;

            var games = applicationDbContext.Games.Include(g => g.Categories).AsQueryable();

            switch (sortOrder)
            {
                case "name_asc":
                    games = games.OrderBy(g => g.Name);
                    break;
                case "name_desc":
                    games = games.OrderByDescending(g => g.Name);
                    break;
                case "price_asc":
                    games = games.OrderBy(g => g.Price);
                    break;
                case "price_desc":
                    games = games.OrderByDescending(g => g.Price);
                    break;
            }

            return View(await games.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
