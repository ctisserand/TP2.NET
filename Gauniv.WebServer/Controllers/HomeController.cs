using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommunityToolkit.HighPerformance;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using X.PagedList.Extensions;
using static Gauniv.WebServer.Data.Game;
using static Gauniv.WebServer.Data.Game.Category;



namespace Gauniv.WebServer.Controllers
{
    public class HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, UserManager<User> userManager) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly ApplicationDbContext applicationDbContext = applicationDbContext;
        private readonly UserManager<User> userManager = userManager;




        public async Task<IActionResult> Index(string sortOrder, string name, decimal? minPrice, decimal? maxPrice, List<int> selectedCategories)
        {
            ViewData["CurrentSort"] = sortOrder;

            var games = applicationDbContext.Games.Include(g => g.Categories).AsQueryable();
            var user = await userManager.GetUserAsync(User);

            List<int> purchasedGames = new List<int>();
            List<Game.Category> allCategories = await applicationDbContext.Categories.ToListAsync();

            if (user != null)
            {
                purchasedGames = await applicationDbContext.Users
                    .Where(u => u.Id == user.Id)
                    .SelectMany(u => u.PurchasedGames)
                    .Select(g => g.Id)
                    .ToListAsync();
            }

            if (!string.IsNullOrEmpty(name))
            {
                games = games.Where(g => g.Name.Contains(name));
            }

            if (minPrice.HasValue)
            {
                games = games.Where(g => g.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                games = games.Where(g => g.Price <= maxPrice);
            }

            if (selectedCategories != null && selectedCategories.Any())
            {
                // Filtrage par catégories
                games = games.Where(g => g.Categories.Any(c => selectedCategories.Contains(c.Id)));
            }



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

            ViewData["Categories"] = allCategories;
            ViewData["PurchasedGames"] = purchasedGames;


            return View(await games.ToListAsync());
        }

        [HttpPost]
        [Authorize] // Seuls les utilisateurs connectés peuvent acheter
        public async Task<IActionResult> Purchase(int gameId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var game = await applicationDbContext.Games.FindAsync(gameId);
            if (game == null)
            {
                return NotFound();
            }

            // Vérifie si l'utilisateur possède déjà le jeu
            if (!user.PurchasedGames.Any(g => g.Id == gameId))
            {
                user.PurchasedGames.Add(game);
                await applicationDbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }

   
}
