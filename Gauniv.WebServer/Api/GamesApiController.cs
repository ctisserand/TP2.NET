using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers
{
    [Route("api/1.0.0/[controller]")]
    [ApiController]
    public class GamesApiController : ControllerBase // Changer le nom du contrôleur ici
    {
        private readonly ApplicationDbContext _context;

        public GamesApiController(ApplicationDbContext context) // Correspond au nouveau nom
        {
            _context = context;
        }

        // GET: api/1.0.0/games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames(
            int offset = 0,
            int limit = 10,
            int? category = null)
        {
            // Construire la requête de base
            var gamesQuery = _context.Games.Include(g => g.Categories).AsQueryable();

            // Filtrer par catégorie si nécessaire
            if (category.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.Categories.Any(c => c.Id == category));
            }

            // Appliquer la pagination
            var totalGamesCount = await gamesQuery.CountAsync();
            var games = await gamesQuery
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            var gameDtos = games.Select(game => new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                Price = game.Price,
                Categories = game.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            }).ToList();
            // Retourner la réponse avec les jeux paginés et le nombre total
            return Ok(new
            {
                TotalCount = totalGamesCount,
                Games = gameDtos
            });
        }
    }
}
