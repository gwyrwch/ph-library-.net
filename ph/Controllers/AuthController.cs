using Microsoft.AspNetCore.Mvc;

namespace ph.Controllers
{
    public class AuthController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return Redirect("Auth/Registration");
        }

        public IActionResult Registration()
        {
            return View();
        }
    }
}