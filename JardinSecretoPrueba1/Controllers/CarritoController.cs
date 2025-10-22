using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

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

        // Enviar pedido por WhatsApp (mejorado)
        public IActionResult EnviarPedidoWhatsApp()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any()) return RedirectToAction("Index");

            // Recuperar datos del cliente desde Session
            var nombre = HttpContext.Session.GetString("NombreUsuario") ?? "Invitado";
            var apellido = HttpContext.Session.GetString("ApellidoUsuario") ?? "";
            var telefono = HttpContext.Session.GetString("TelefonoUsuario") ?? "";
            var tipoPedido = HttpContext.Session.GetString("TipoPedido") ?? "No especificado";
            var horaPedido = HttpContext.Session.GetString("HoraPedido") ?? "";
            var direccion = HttpContext.Session.GetString("DireccionUsuario") ?? "";

            var nombreCompleto = string.IsNullOrWhiteSpace(apellido) ? nombre : $"{nombre} {apellido}";

            // Construimos mensaje
            var sb = new StringBuilder();
            sb.AppendLine("*Nuevo pedido*");
            sb.AppendLine($"*Cliente:* {nombreCompleto}");
            if (!string.IsNullOrWhiteSpace(telefono))
                sb.AppendLine($"*Teléfono:* {telefono}");
            sb.AppendLine($"*Tipo de pedido:* {tipoPedido}");
            if (!string.IsNullOrWhiteSpace(horaPedido) && tipoPedido != "Delivery")
                sb.AppendLine($" *Hora:* {horaPedido}");
            if (tipoPedido == "Delivery" && !string.IsNullOrWhiteSpace(direccion))
                sb.AppendLine($" *Dirección:* {direccion}");
            sb.AppendLine(new string('-', 30));
            sb.AppendLine("Pedido:");

            // ... (código anterior del método) ...

            decimal total = 0m;

            // Recorremos el carrito. 'p' es un objeto de tipo "CarritoItem".
            foreach (var p in carrito)
            {
                // 1. Usamos la propiedad 'Subtotal' del CarritoItem. 
                // Esta ya incluye el precio base + sabor + extras, todo multiplicado por la cantidad.
                total += p.Subtotal;

                // 2. Construimos el texto del producto
                sb.AppendLine($"- {p.Nombre} x{p.Cantidad}"); // Ej: - Torta x1

                // 3. Añadimos el sabor (si tiene un nombre)
                if (!string.IsNullOrEmpty(p.SaborNombre))
                {
                    // Usamos sangría para que se vea ordenado en WhatsApp
                    sb.AppendLine($"    Sabor: {p.SaborNombre}"); // Ej: Sabor: Carne
                }

                // 4. Añadimos los extras (si hay alguno)
                if (p.Extras != null && p.Extras.Any())
                {
                    // Unimos los nombres de los extras con comas
                    var nombresExtras = string.Join(", ", p.Extras.Select(e => e.Nombre));
                    sb.AppendLine($"    Extras: {nombresExtras}"); // Ej: Extras: Salsita, Queso
                }

                // 5. Añadimos el subtotal de ESE item
                sb.AppendLine($"    Subtotal: ${p.Subtotal.ToString("0.00", CultureInfo.InvariantCulture)}");
            }

            sb.AppendLine(new string('-', 30));
            // Usamos 'total' (la suma de todos los subtotales) para el gran total
            sb.AppendLine($"*Total a pagar:* ${total.ToString("0.00", CultureInfo.InvariantCulture)}");

            // Número de WhatsApp del negocio — debe incluir código de país y SIN el '+', SIN espacios.
            // Ejemplo para El Salvador: "50312345678"
            var numeroWhatsApp = "50370857606"; // <-- reemplaza por tu número real (p.ej. "50312345678")

            var url = $"https://wa.me/{numeroWhatsApp}?text={Uri.EscapeDataString(sb.ToString())}";

            // Vaciamos el carrito de la sesión ANTES de redirigir
            HttpContext.Session.Remove("Carrito");

            return Redirect(url);
        }

        // Sumar una unidad de un producto existente
        public IActionResult Sumar(int id)
        {
            var carrito = ObtenerCarrito();

            var producto = carrito.FirstOrDefault(p => p.ProductoId == id);
            if (producto != null)
            {
                producto.Cantidad += 1; // aumenta una unidad
                GuardarCarrito(carrito);
            }

            return RedirectToAction("Index");
        }

        // Restar una unidad de un producto existente
        public IActionResult Restar(int id)
        {
            var carrito = ObtenerCarrito();

            var producto = carrito.FirstOrDefault(p => p.ProductoId == id);
            if (producto != null)
            {
                if (producto.Cantidad > 1)
                {
                    producto.Cantidad -= 1; // resta una unidad
                }
                else
                {
                    // Si llega a 0, se elimina del carrito
                    carrito.Remove(producto);
                }

                GuardarCarrito(carrito);
            }

            return RedirectToAction("Index");
        }


        // -------------------------------------
        // Helpers privados (Métodos ayudantes)
        // -------------------------------------
        private List<CarritoItem> ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson))
                return new List<CarritoItem>();
            return JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson) ?? new List<CarritoItem>();
        }

        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
        }

        [HttpPost]
        public async Task<IActionResult> AgregarConOpciones([FromBody] CarritoItem item)
        {
            if (item == null || item.ProductoId <= 0)
            {
                return BadRequest("Datos del producto inválidos."); // Añadimos una validación
            }
            var producto = await _context.Productos.FindAsync(item.ProductoId);

            if (producto == null)
            {
                return NotFound("Producto no encontrado."); // Otra validación
            }

            item.Nombre = producto.Nombre;
            item.Precio = producto.Precio;
            item.ImagenUrl = producto.ImagenUrl;

            var carrito = ObtenerCarrito();

            carrito.Add(item);

            GuardarCarrito(carrito);

            return Ok(new { cantidad = carrito.Count });
        }
    }

}
