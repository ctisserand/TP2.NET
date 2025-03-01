using Microsoft.AspNetCore.Mvc;

namespace Gauniv.WebServer.Services
{
    public class UserStatusService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
