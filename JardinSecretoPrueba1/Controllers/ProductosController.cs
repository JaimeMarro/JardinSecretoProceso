using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Authorization;

namespace JardinSecretoPrueba1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductosController : Controller
    {
        private readonly JardinSecretoContext _context;

        public ProductosController(JardinSecretoContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var jardinSecretoContext = _context.Productos.Include(p => p.Categoria);
            return View(await jardinSecretoContext.ToListAsync());
        }

        public IActionResult ErrorDelete()
        {
            return View();
        }

        // GET: Productos/Details/5
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

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Nombre");
            
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductoId,Nombre,Descripcion,Precio,Disponible,ImagenUrl,CategoriaId, archivoAdjunto")] Producto producto, IFormFile imagen)
        {
            
            if (imagen != null && imagen.Length > 0)
            {
                //Con Guid.NewGuid creamos un nombre unico para la imagen, Path.GetExtension borra el nombre que contiene la imagen solo dejando el .png, .jpg etc
                var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";

                //En este apartado Path.Combine, comobina las direcciones para crear la ruta, Directory.GetCurrentDirectory es para obtener la ruta en la que nos encontramos
                //wwwroot, imagenes y nombreArchivo es lo demas que le combinamos para que se convierta en una ruta
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", nombreArchivo);

                //En pocas palabras guarda la imagen
                await using (var stream = new FileStream(ruta, FileMode.Create))
                    await imagen.CopyToAsync(stream);

                //Guardamos la ruta en la base de datos, en la tabla productom campo ImagenUrl
                producto.ImagenUrl = $"/imagenes/{nombreArchivo}";
            }
            else
            {
                ModelState.AddModelError("ImagenUrl", "Debe seleccionar una imagen valida");
                
            }

            if (!ModelState.IsValid)
            {
                ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
                return View(producto);
            }

            //Guarda los demas datos y nos dirije al index
            _context.Add(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Productos");
        }




        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoId,Nombre,Descripcion,Precio,Disponible,ImagenUrl,CategoriaId")] Producto producto, IFormFile imagen)
        {
            if (id != producto.ProductoId)
            {
                return NotFound();
            }
            var productoExistente = await _context.Productos.FindAsync(id);

            if (productoExistente == null)
            {
                return NotFound();
            }
            //Nos aseguramos que se pueda actualizar la imagen
            if (imagen != null && imagen.Length > 0)
            {
                //Con Guid.NewGuid creamos un nombre unico para la imagen, Path.GetExtension borra el nombre que contiene la imagen solo dejando el .png, .jpg etc
                var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";

                //En este apartado Path.Combine, comobina las direcciones para crear la ruta, Directory.GetCurrentDirectory es para obtener la ruta en la que nos encontramos
                //wwwroot, imagenes y nombreArchivo es lo demas que le combinamos para que se convierta en una ruta
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", nombreArchivo);

                //En pocas palabras guarda la imagen
                await using (var stream = new FileStream(ruta, FileMode.Create))
                    await imagen.CopyToAsync(stream);

                //Guardamos la ruta en la base de datos, en la tabla productom campo ImagenUrl
                productoExistente.ImagenUrl = $"/imagenes/{nombreArchivo}";
            }

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.Disponible = producto.Disponible;
            productoExistente.CategoriaId = producto.CategoriaId;

            ModelState.Remove("Imagen");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productoExistente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto != null)
                {
                    _context.Productos.Remove(producto);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return View("ErrorDelete");
            }
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}
