using Microsoft.AspNetCore.Mvc;

namespace JardinSecretoPrueba1.Controllers
{
    public class CarritoController : Controller
    {
        public IActionResult Index()
        {
            var nombre = HttpContext.Session.GetString("NombreUsuario") ?? "Invitado";
            var pedido = HttpContext.Session.GetString("TipoPedido") ?? "No especificado";

            ViewBag.Nombre = nombre;
            ViewBag.TipoPedido = pedido;

            return View();
        }

    }
}
