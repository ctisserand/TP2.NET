using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers
{
    [Route("api/1.0.0/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]  // Limiter l'accès aux utilisateurs ayant le rôle "Admin"
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
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
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

        // GET: api/Game/{id}/payload
        [HttpGet("LinkToDownloadPayload/{id}")]
        public async Task<IActionResult> GetLinkToDownloadPayload(int id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var game = await _context.Games
                                      .AsNoTracking()  // Optimisation pour la lecture
                                      .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            // Spécifier le chemin du fichier local où tu souhaites sauvegarder le payload
            var localFilePath = Path.Combine(@"C:\", $"game_{id}_payload.bin");
            /*
            // Créer le fichier localement en écrivant le contenu du Payload en streaming
            using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.WriteAsync(game.Payload, 0, game.Payload.Length);
            }

            // Retourner le fichier local créé, ou un autre message si tu préfères
            return Ok(new { message = "File successfully saved locally", path = localFilePath });*/

            // Vérifier si le payload existe
            if (game.Payload == null || game.Payload.Length == 0)
            {
                return BadRequest("Payload is empty or missing.");
            }

            // Retourner le fichier en tant que réponse HTTP pour que le client puisse le télécharger
            return File(game.Payload, "application/octet-stream", $"game_{id}.exe");
        }

        // GET: api/Game/{id}/payload
        [HttpGet("SavePayloadLocally/{id}")]
        [Authorize]  // Vérifie que l'utilisateur est bien connecté
        public async Task<IActionResult> SavePayloadLocally(int id)
        {
            var game = await _context.Games
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            if (game.Payload == null || game.Payload.Length == 0)
            {
                return BadRequest("Payload is empty or missing.");
            }

            // Chemin où enregistrer le fichier (MODIFIE-LE si besoin)
            var savePath = Path.Combine(@"C:\Games\", $"game_{id}.exe");

            try
            {
                // Créer le répertoire s'il n'existe pas
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                // Sauvegarde du fichier
                await System.IO.File.WriteAllBytesAsync(savePath, game.Payload);

                return Ok(new { message = "Payload saved successfully", path = savePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving payload: {ex.Message}");
            }
        }

    }
}
