using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;


        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;

            var games = _context.Games.Include(g => g.Categories).AsQueryable();

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

        // GET: Games/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Games/Create using AddGameDTO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddGameDTO addGameDto)
        {
            if (ModelState.IsValid)
            {
                var game = new Game
                {
                    Name = addGameDto.Name,
                    Description = addGameDto.Description,
                    Price = addGameDto.Price,
                    // Si aucun fichier n'est fourni, on stocke un tableau vide
                    Payload = (addGameDto.Payload != null && addGameDto.Payload.Length > 0)
                                ? await ConvertFileToBytes(addGameDto.Payload)
                                : new byte[0]
                };

                // Traitement des catégories
                foreach (var catName in addGameDto.Categories)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == catName);
                    if (category == null)
                    {
                        category = new Game.Category { Name = catName };
                        _context.Categories.Add(category);
                    }
                    game.Categories.Add(category);
                }

                _context.Games.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(addGameDto);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var game = await _context.Games.Include(g => g.Categories).FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
                return NotFound();

            // Mapper l'entité vers le DTO pour pré-remplir le formulaire
            var editGameDto = new EditGameDTO
            {
                Name = game.Name,
                Description = game.Description,
                Price = game.Price,
                Categories = game.Categories.Select(c => c.Name).ToList(),
                // Le payload actuel n'est pas envoyé dans le formulaire, on le garde en mémoire côté serveur.
                // L'utilisateur pourra uploader un nouveau fichier s'il le souhaite.
                Payload = null
            };

            return View(editGameDto);
        }

        // POST: Games/Edit/5 using EditGameDTO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditGameDTO editGameDto)
        {
            // On retire l'erreur de validation liée à Payload pour autoriser une édition sans modification du fichier
            //ModelState.Remove("Payload");

            var game = await _context.Games.Include(g => g.Categories)
                                           .FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Mise à jour des propriétés de base
                game.Name = editGameDto.Name;
                game.Description = editGameDto.Description;
                game.Price = editGameDto.Price;

                // Si un nouveau fichier est uploadé, mettre à jour le payload ; sinon, conserver l'ancien
                if (editGameDto.Payload != null && editGameDto.Payload.Length > 0)
                {
                    game.Payload = await ConvertFileToBytes(editGameDto.Payload);
                }

                // Récupérer la liste distincte des noms de catégories du DTO
                var desiredCategoryNames = editGameDto.Categories.Distinct().ToList();

                // Charger en une seule requête toutes les catégories existantes qui correspondent aux noms désirés
                var existingCategories = await _context.Categories
                    .Where(c => desiredCategoryNames.Contains(c.Name))
                    .ToListAsync();

                // Déterminer les noms manquants (catégories non présentes en BDD)
                var missingCategoryNames = desiredCategoryNames
                    .Except(existingCategories.Select(c => c.Name))
                    .ToList();

                // Créer les catégories manquantes et les ajouter au contexte
                var newCategories = missingCategoryNames
                    .Select(name => new Game.Category { Name = name })
                    .ToList();
                if (newCategories.Any())
                {
                    _context.Categories.AddRange(newCategories);
                }

                // Mettre à jour la collection de catégories du jeu
                // Ici, on regroupe les catégories existantes et celles nouvellement créées
                var updatedCategories = existingCategories.Concat(newCategories).ToList();

                // Remplacer les catégories associées au jeu
                game.Categories.Clear();
                foreach (var category in updatedCategories)
                {
                    game.Categories.Add(category);
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Games.Any(g => g.Id == game.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editGameDto);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var game = await _context.Games.Include(g => g.Categories)
                                           .FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
                return NotFound();

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(Game g)
        {
            return g != null;
        }

        // Méthode utilitaire pour convertir un fichier (FormFile) en tableau de bytes
        private async Task<byte[]> ConvertFileToBytes(Microsoft.AspNetCore.Http.IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
