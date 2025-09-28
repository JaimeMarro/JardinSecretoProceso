using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JardinSecretoPrueba1.Controllers
{
    public class ProductosClientesController : Controller
    {
        private readonly JardinSecretoContext _context;

        public ProductosClientesController(JardinSecretoContext context)
        {
            _context = context;
        }

        // GET: muestra el formulario
        [HttpGet]
        public IActionResult NombreUsuario()
        {
            return View();
        }

        // POST: recibe datos del formulario
        [HttpPost]
        public IActionResult NombreUsuario(string nombreusuario, string tipoPedido)
        {
            HttpContext.Session.SetString("NombreUsuario", nombreusuario);
            HttpContext.Session.SetString("TipoPedido", tipoPedido);

            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Index()
        {
            var nombre = HttpContext.Session.GetString("NombreUsuario") ?? "Invitado";
            var pedido = HttpContext.Session.GetString("TipoPedido") ?? "No especificado";

            ViewBag.Nombre = nombre;
            ViewBag.TipoPedido = pedido;

            var jardinSecretoContext = _context.Productos.Include(p => p.Categoria);
            return View(await jardinSecretoContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        //Configuracion del carrito Carrito
        public IActionResult Carrito()
        {
            return View();
        }
    }
}
