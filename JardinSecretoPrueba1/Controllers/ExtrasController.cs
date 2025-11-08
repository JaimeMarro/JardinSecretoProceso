using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JardinSecretoPrueba1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExtrasController : Controller
    {
        private readonly JardinSecretoContext _context;

        public ExtrasController(JardinSecretoContext context)
        {
            _context = context;
        }

        // GET: Extras (CORREGIDO)
        // Carga la información del producto relacionado con .Include()
        public async Task<IActionResult> Index()
        {
            var jardinSecretoContext = _context.Extras.Include(e => e.IdProductoNavigation);
            return View(await jardinSecretoContext.ToListAsync());
        }

        // GET: Extras/Details/5 (CORREGIDO)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extra = await _context.Extras
                .Include(e => e.IdProductoNavigation) // <-- CORRECCIÓN
                .FirstOrDefaultAsync(m => m.ExtraId == id);
            if (extra == null)
            {
                return NotFound();
            }

            return View(extra);
        }

        // GET: Extras/Create (CORREGIDO)
        public IActionResult Create()
        {
            ViewData["IdProducto"] = new SelectList(_context.Productos, "ProductoId", "Nombre");
            return View();
        }

        // POST: Extras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExtraId,Nombre,PrecioExtra,IdProducto")] Extra extra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(extra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "ProductoId", "Nombre", extra.IdProducto);
            return View(extra);
        }

        // GET: Extras/Edit/5 (CORREGIDO)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extra = await _context.Extras.FindAsync(id);
            if (extra == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "ProductoId", "Nombre", extra.IdProducto);
            return View(extra);
        }

        // POST: Extras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExtraId,Nombre,PrecioExtra,IdProducto")] Extra extra)
        {
            if (id != extra.ExtraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(extra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExtraExists(extra.ExtraId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "ProductoId", "Nombre", extra.IdProducto);
            return View(extra);
        }

        // GET: Extras/Delete/5 (CORREGIDO)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extra = await _context.Extras
                .Include(e => e.IdProductoNavigation) // <-- CORRECCIÓN
                .FirstOrDefaultAsync(m => m.ExtraId == id);
            if (extra == null)
            {
                return NotFound();
            }

            return View(extra);
        }

        // POST: Extras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var extra = await _context.Extras.FindAsync(id);
            if (extra != null)
            {
                _context.Extras.Remove(extra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExtraExists(int id)
        {
            return _context.Extras.Any(e => e.ExtraId == id);
        }
    }
}