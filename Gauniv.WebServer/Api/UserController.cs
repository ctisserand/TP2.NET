using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Gauniv.WebServer.Data; // Assurez-vous d'importer votre modèle User

namespace Gauniv.WebServer.Api
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users.ToList();
            var userList = users.Select(u => new
            {
                name = u.UserName,
                isOnline = false // ⚠️ Ajoute une gestion de statut si besoin, car User n'a pas "IsOnline"
            }).ToList();

            return Ok(userList);
        }
    }
}
