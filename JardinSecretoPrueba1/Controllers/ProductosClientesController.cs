using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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

        // POST: recibe datos del formulario (ahora con apellido, teléfono y dirección)
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
            // Guardamos en Session (usamos valores vacíos si vienen null para evitar NRE)
            HttpContext.Session.SetString("NombreUsuario", nombreUsuario ?? "");
            HttpContext.Session.SetString("ApellidoUsuario", apellidoUsuario ?? "");
            HttpContext.Session.SetString("TelefonoUsuario", telefonoUsuario ?? "");
            HttpContext.Session.SetString("TipoPedido", tipoPedido ?? "");

            // Si es Delivery, guardamos la dirección; si no, removemos cualquier dirección previa
            if (!string.IsNullOrWhiteSpace(tipoPedido) && tipoPedido == "Delivery")
            {
                HttpContext.Session.SetString("DireccionUsuario", direccionUsuario ?? "");
            }
            else
            {
                HttpContext.Session.Remove("DireccionUsuario");
            }

            if (!string.IsNullOrWhiteSpace(tipoPedido) && tipoPedido != "Delivery" && !string.IsNullOrEmpty(horaPedido))
            {
                HttpContext.Session.SetString("HoraPedido", horaPedido);
            }
            else
            {
                HttpContext.Session.Remove("HoraPedido");
            }

            return RedirectToAction("Index");
        }

        // Index: leemos la session y llenamos ViewBag para la vista
        public async Task<IActionResult> Index()
        {
            var nombre = HttpContext.Session.GetString("NombreUsuario") ?? "Invitado";
            var apellido = HttpContext.Session.GetString("ApellidoUsuario") ?? "";
            var nombreCompleto = string.IsNullOrWhiteSpace(apellido) ? nombre : $"{nombre} {apellido}";

            var pedido = HttpContext.Session.GetString("TipoPedido") ?? "No especificado";
            var telefono = HttpContext.Session.GetString("TelefonoUsuario") ?? "";
            var direccion = HttpContext.Session.GetString("DireccionUsuario") ?? "";
            var horaPedido = HttpContext.Session.GetString("HoraPedido") ?? "";

            ViewBag.Nombre = nombreCompleto;
            ViewBag.TipoPedido = pedido;
            ViewBag.Telefono = telefono;
            ViewBag.Direccion = direccion;
            ViewBag.HoraPedido = horaPedido;

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

        // GET: Devuelve sabores y extras para llenar el modal (DTOs simples)
        [HttpGet]
        public async Task<IActionResult> GetOpciones(int id)
        {
            var sabores = await _context.Sabors
                .Where(s => s.IdProducto == id)
                .Select(s => new {
                    id = s.SaborId,
                    nombre = s.Nombre,
                    precio = s.PrecioSabor
                })
                .ToListAsync();

            var extras = await _context.Extras
                .Where(e => e.IdProducto == id)
                .Select(e => new {
                    id = e.ExtraId,
                    nombre = e.Nombre,
                    precio = e.PrecioExtra
                })
                .ToListAsync();

            return Json(new { sabores, extras });
        }





    }
}
