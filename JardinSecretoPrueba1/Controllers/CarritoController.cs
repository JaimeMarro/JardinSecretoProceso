using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JardinSecretoPrueba1.Controllers
{
    public class CarritoController : Controller
    {
        private readonly JardinSecretoContext _context;

        public CarritoController(JardinSecretoContext context)
        {
            _context = context;
        }

        // Mostrar carrito
        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            return View(carrito);
        }

        // Agregar producto al carrito
        public IActionResult Agregar(int id)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.ProductoId == id);
            if (producto != null)
            {
                var carrito = ObtenerCarrito();

                // Verificar si ya existe en el carrito
                var itemExistente = carrito.FirstOrDefault(p => p.ProductoId == id);
                if (itemExistente != null)
                {
                    itemExistente.Cantidad += 1; // Sumar cantidad
                }
                else
                {
                    producto.Cantidad = 1; // inicializar cantidad
                    carrito.Add(producto);
                }

                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index", "ProductosClientes");
        }

        // Quitar producto del carrito
        public IActionResult Quitar(int id)
        {
            var carrito = ObtenerCarrito();
            var producto = carrito.FirstOrDefault(p => p.ProductoId == id);
            if (producto != null)
            {
                carrito.Remove(producto);
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }

        // Vaciar carrito
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove("Carrito");
            return RedirectToAction("Index");
        }

        // Enviar pedido por WhatsApp
        public IActionResult EnviarPedidoWhatsApp()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any()) return RedirectToAction("Index");

            var mensaje = "Hola, me gustaría pedir:\n";
            foreach (var p in carrito)
            {
                mensaje += $"- {p.Nombre} x{p.Cantidad} (${p.Precio * p.Cantidad})\n";
            }

            // Número de WhatsApp
            var url = $"https://wa.me/50370857606?text={Uri.EscapeDataString(mensaje)}";

            return Redirect(url);
        }

        // ------------------------
        // Helpers privados
        // ------------------------
        private List<Producto> ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson))
                return new List<Producto>();
            return JsonConvert.DeserializeObject<List<Producto>>(carritoJson) ?? new List<Producto>();
        }

        private void GuardarCarrito(List<Producto> carrito)
        {
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
        }
    }
}
