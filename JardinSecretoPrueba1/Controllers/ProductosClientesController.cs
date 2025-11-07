using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq; // <-- Asegúrate de tener este 'using'

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
        [ActionName("NombreUsuario")]
        public IActionResult NombreUsuario(
            string nombreUsuario,
            string apellidoUsuario,
            string telefonoUsuario,
            string tipoPedido,
            string? direccionUsuario,
            string? horaPedido)
        {
            // ... (Tu código para manejar la sesión se mantiene igual) ...
            HttpContext.Session.SetString("NombreUsuario", nombreUsuario ?? "");
            HttpContext.Session.SetString("ApellidoUsuario", apellidoUsuario ?? "");
            HttpContext.Session.SetString("TelefonoUsuario", telefonoUsuario ?? "");
            HttpContext.Session.SetString("TipoPedido", tipoPedido ?? "");

            if (!string.IsNullOrWhiteSpace(tipoPedido) && tipoPedido == "Delivery")
            {
                HttpContext.Session.SetString("DireccionUsuario", direccionUsuario ?? "");
            }
            else { HttpContext.Session.Remove("DireccionUsuario"); }

            if (!string.IsNullOrWhiteSpace(tipoPedido) && tipoPedido != "Delivery" && !string.IsNullOrEmpty(horaPedido))
            {
                HttpContext.Session.SetString("HoraPedido", horaPedido);
            }
            else { HttpContext.Session.Remove("HoraPedido"); }

            return RedirectToAction("Index");
        }

        // Index: Muestra la lista de productos
        public async Task<IActionResult> Index()
        {
            var nombre = HttpContext.Session.GetString("NombreUsuario") ?? "Invitado";
            var apellido = HttpContext.Session.GetString("ApellidoUsuario") ?? "";
            var nombreCompleto = string.IsNullOrWhiteSpace(apellido) ? nombre : $"{nombre} {apellido}";

            ViewBag.Nombre = nombreCompleto;
            ViewBag.TipoPedido = HttpContext.Session.GetString("TipoPedido") ?? "No especificado";
            ViewBag.Direccion = HttpContext.Session.GetString("DireccionUsuario") ?? "";
            ViewBag.HoraPedido = HttpContext.Session.GetString("HoraPedido") ?? "";

            var productos = await _context.Productos.Include(p => p.Categoria).ToListAsync();
            return View(productos);
        }

        // ===== INICIO DE LA CORRECCIÓN DEFINITIVA =====

        [HttpGet]
        public async Task<IActionResult> GetOpcionesProducto(int productoId)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return NotFound();

            var sabores = await _context.Sabors // <-- CORREGIDO: 'Sabors'
                .Where(s => s.IdProducto == productoId) // <-- CORREGIDO: 'IdProducto'
                .Select(s => new {
                    id = s.SaborId,
                    nombre = s.Nombre,
                    precio = s.PrecioSabor
                })
                .ToListAsync();

            var extras = await _context.Extras
                .Where(e => e.IdProducto == productoId) // <-- CORREGIDO: 'IdProducto'
                .Select(e => new {
                    id = e.ExtraId,
                    nombre = e.Nombre,
                    precio = e.PrecioExtra // <-- CORREGIDO: 'PrecioExtra'
                })
                .ToListAsync();

            return Json(new
            {
                nombreProducto = producto.Nombre,
                sabores,
                extras
            });
        }

        // ===== FIN DE LA CORRECCIÓN =====

        // El resto de tus métodos
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        public IActionResult Carrito()
        {
            return View();
        }
    }
}