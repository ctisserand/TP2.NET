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

namespace Gauniv.WebServer.Controllers
{
    public class GameController(ILogger<GameController> logger, ApplicationDbContext applicationDbContext, UserManager<User> userManager) : Controller
    {
        private readonly ILogger<GameController> _logger = logger;
        private readonly ApplicationDbContext applicationDbContext = applicationDbContext;
        private readonly UserManager<User> userManager = userManager;

        private readonly ApplicationDbContext _context = applicationDbContext;

        [HttpGet]
        public IActionResult CreateGame()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(CreateGameViewModel model, IFormFile? payloadFile)
        {
            if (ModelState.IsValid)
            {
                byte[]? fileBytes = null;
                if (payloadFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        payloadFile.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                }

                Game game = new Game
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    //Categories = model.Categories != null ? string.Join(",", model.Categories) : null,
                    Payload = fileBytes
                };

                _context.Games.Add(game);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
