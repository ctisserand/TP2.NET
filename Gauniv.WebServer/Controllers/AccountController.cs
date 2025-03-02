using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Gauniv.WebServer.Data; // Assure-toi d'inclure ton modèle User
using Gauniv.WebServer.Models; // Assure-toi que LoginViewModel est bien défini

namespace Gauniv.WebServer.Controllers
{
    /*public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // ✅ Met à jour le statut en ligne
                    user.IsOnline = true;
                    await _userManager.UpdateAsync(user);

                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Identifiants invalides.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // ✅ Met à jour le statut hors ligne
                user.IsOnline = false;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }*/
}
