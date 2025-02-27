using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gauniv.WebServer.Data;
using System.Linq;
using System.Threading.Tasks;

[Authorize] // Empêche les utilisateurs non connectés d'accéder à cette page
public class LibraryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public LibraryController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> LybraryIndex(string sortOrder)
    {
        var user = await _userManager.Users
            .Include(u => u.PurchasedGames)
            .ThenInclude(g => g.Categories)
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (user == null)
        {
            return RedirectToAction("Login", "Account"); // Redirige vers la page de connexion si l'utilisateur est inconnu
        }

        var games = user.PurchasedGames.AsQueryable();
        ViewData["CurrentSort"] = sortOrder;

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

        return View(games.ToList());
    }
}
