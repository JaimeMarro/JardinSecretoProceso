using Microsoft.AspNetCore.Mvc;

namespace JardinSecretoPrueba1.Controllers
{
    public class CarritoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
