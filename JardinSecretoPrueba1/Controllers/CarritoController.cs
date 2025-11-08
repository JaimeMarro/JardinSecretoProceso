using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JardinSecretoPrueba1.Controllers
{
    public class CarritoController : Controller
    {
        private readonly JardinSecretoContext _context;

        public CarritoController(JardinSecretoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            return View(carrito);
        }
        public IActionResult Quitar(Guid id)
        {
            var carrito = ObtenerCarrito();
            var itemEnCarrito = carrito.FirstOrDefault(p => p.CartItemId == id);
            if (itemEnCarrito != null)
            {
                carrito.Remove(itemEnCarrito);
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove("Carrito");
            return RedirectToAction("Index");
        }
        public IActionResult EnviarPedidoWhatsApp()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any()) return RedirectToAction("Index");
            var nombre = HttpContext.Session.GetString("NombreUsuario") ?? "Invitado";
            var apellido = HttpContext.Session.GetString("ApellidoUsuario") ?? "";
            var telefono = HttpContext.Session.GetString("TelefonoUsuario") ?? "";
            var tipoPedido = HttpContext.Session.GetString("TipoPedido") ?? "No especificado";
            var horaPedido = HttpContext.Session.GetString("HoraPedido") ?? "";
            var direccion = HttpContext.Session.GetString("DireccionUsuario") ?? "";
            var nombreCompleto = string.IsNullOrWhiteSpace(apellido) ? nombre : $"{nombre} {apellido}";
            var sb = new StringBuilder();
            sb.AppendLine("*Nuevo pedido*");
            sb.AppendLine($"*Cliente:* {nombreCompleto}");
            if (!string.IsNullOrWhiteSpace(telefono)) sb.AppendLine($"*Teléfono:* {telefono}");
            sb.AppendLine($"*Tipo de pedido:* {tipoPedido}");
            if (!string.IsNullOrWhiteSpace(horaPedido) && tipoPedido != "Delivery") sb.AppendLine($" *Hora:* {horaPedido}");
            if (tipoPedido == "Delivery" && !string.IsNullOrWhiteSpace(direccion)) sb.AppendLine($" *Dirección:* {direccion}");
            sb.AppendLine(new string('-', 30));
            sb.AppendLine("Pedido:");
            decimal total = 0m;
            foreach (var p in carrito)
            {
                total += p.Subtotal;
                sb.AppendLine($"- {p.Nombre} x{p.Cantidad}");
                if (!string.IsNullOrEmpty(p.SaborNombre))
                {
                    sb.AppendLine($"    Sabor: {p.SaborNombre}");
                }
                if (p.Extras != null && p.Extras.Any())
                {
                    var nombresExtras = string.Join(", ", p.Extras.Select(e => $"{e.Nombre} (+${e.Precio.ToString("0.00")})"));
                    sb.AppendLine($"    Extras: {nombresExtras}");
                }
                sb.AppendLine($"    Subtotal: ${p.Subtotal.ToString("0.00", CultureInfo.InvariantCulture)}");
            }
            sb.AppendLine(new string('-', 30));
            sb.AppendLine($"*Total a pagar:* ${total.ToString("0.00", CultureInfo.InvariantCulture)}");
            var numeroWhatsApp = "50370857606";
            var url = $"https://wa.me/{numeroWhatsApp}?text={Uri.EscapeDataString(sb.ToString())}";
            HttpContext.Session.Remove("Carrito");
            return Redirect(url);
        }
        public IActionResult Sumar(Guid id)
        {
            var carrito = ObtenerCarrito();
            var itemEnCarrito = carrito.FirstOrDefault(p => p.CartItemId == id);
            if (itemEnCarrito != null)
            {
                itemEnCarrito.Cantidad += 1;
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Restar(Guid id)
        {
            var carrito = ObtenerCarrito();
            var itemEnCarrito = carrito.FirstOrDefault(p => p.CartItemId == id);
            if (itemEnCarrito != null)
            {
                if (itemEnCarrito.Cantidad > 1) { itemEnCarrito.Cantidad -= 1; }
                else { carrito.Remove(itemEnCarrito); }
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }
        private List<CarritoItem> ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson)) return new List<CarritoItem>();
            return JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson) ?? new List<CarritoItem>();
        }
        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
        }

        // ===== INICIO DE LA CORRECCIÓN =====
        [HttpPost]
        public async Task<IActionResult> AgregarConOpciones([FromBody] CarritoViewModel itemVM)
        {
            if (itemVM == null || itemVM.ProductoId <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            var producto = await _context.Productos.FindAsync(itemVM.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado.");

            var sabor = itemVM.SaborId > 0 ? await _context.Sabors.FindAsync(itemVM.SaborId) : null;

            var extrasSeleccionados = new List<Extra>();
            if (itemVM.ExtrasIds != null && itemVM.ExtrasIds.Any())
            {
                extrasSeleccionados = await _context.Extras
                    .Where(e => itemVM.ExtrasIds.Contains(e.ExtraId))
                    .ToListAsync();
            }

            var carrito = ObtenerCarrito();

            var nuevoItem = new CarritoItem
            {
                ProductoId = producto.ProductoId,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                ImagenUrl = producto.ImagenUrl,
                Cantidad = 1,
                SaborNombre = sabor?.Nombre,
                SaborPrecio = sabor?.PrecioSabor ?? 0,
                Extras = extrasSeleccionados.Select(e => new ExtraItem
                {
                    ExtraId = e.ExtraId,
                    Nombre = e.Nombre,
                    Precio = e.PrecioExtra // <-- SE CORRIGIÓ LA ASIGNACIÓN
                }).ToList()
            };

            carrito.Add(nuevoItem);
            GuardarCarrito(carrito);

            return Ok(new { message = "Producto agregado correctamente." });
        }
        // ===== FIN DE LA CORRECCIÓN =====
    }
}