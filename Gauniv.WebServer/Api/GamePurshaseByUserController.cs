using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;

namespace Gauniv.WebServer.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamePurchaseByUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GamePurchaseByUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/possessedgame
        // GET: api/possessedgame?offset=0&limit=10
        // GET: api/possessedgame?category=3
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetPurchasedGames(
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10,
            [FromQuery] int? category = null)
        {
            // Récupérer l'utilisateur connecté (à partir du user authentifié)
            var userId = User.Identity.Name;  // Assure-toi que l'utilisateur est authentifié
            var user = await _context.Users
                .Include(u => u.PurchasedGames)
                .ThenInclude(g => g.Categories) // Assurez-vous d'inclure les catégories pour appliquer le filtre
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (user == null)
            {
                return NotFound("Utilisateur non trouvé");
            }

            // Application du filtre par catégorie si nécessaire
            var gamesQuery = _context.Games.AsQueryable();

            if (category.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.Categories.Any(c => c.Id == category.Value));
            }

            // Filtrage des jeux possédés par l'utilisateur
            var purchasedGames = user.PurchasedGames
                .Where(pg => gamesQuery.Contains(pg))  // Filtrer par les jeux possédés
                .Skip(offset)
                .Take(limit)
                .ToList();

            // Convertir les jeux en DTO
            var gameDtos = purchasedGames.Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Price = g.Price,
                Categories = g.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            }).ToList();

            // Créer le UserDto avec les jeux récupérés
            var userDto = new UserDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PurchasedGames = gameDtos
            };

            return Ok(userDto);
        }
    }
}
