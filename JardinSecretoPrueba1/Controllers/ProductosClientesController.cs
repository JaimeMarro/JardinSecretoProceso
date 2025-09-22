using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JardinSecretoPrueba1.Controllers
{
    public class ProductosClientesController : Controller
    {
        private readonly JardinSecretoContext _context;

        String? nombreUsuario = null;
        public ProductosClientesController(JardinSecretoContext context)
        {
            _context = context;
        }
        public IActionResult NombreUsuario(String nombre)
        {
            nombreUsuario = nombre;
            return View();
        }

        public async Task<IActionResult> Index(String nombreUsuario)
        {
            this.nombreUsuario = nombreUsuario;
            ViewBag.Nombre = nombreUsuario;
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
