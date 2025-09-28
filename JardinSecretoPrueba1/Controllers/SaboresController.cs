using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JardinSecretoPrueba1.Models;

namespace JardinSecretoPrueba1.Controllers
{
    public class SaboresController : Controller
    {
        private readonly JardinSecretoContext _context;

        public SaboresController(JardinSecretoContext context)
        {
            _context = context;
        }

        // GET: Sabores
        public async Task<IActionResult> Index()
        {
            var jardinSecretoContext = _context.Sabors.Include(s => s.IdProductoNavigation);
            return View(await jardinSecretoContext.ToListAsync());
        }

        // GET: Sabores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sabor = await _context.Sabors
                .Include(s => s.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.SaborId == id);
            if (sabor == null)
            {
                return NotFound();
            }

            return View(sabor);
        }

        // GET: Sabores/Create
        public IActionResult Create()
        {
            ViewData["IdProducto"] = new SelectList(_context.Productos, "ProductoId", "Nombre");
            return View();
        }

        // POST: Sabores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SaborId,IdProducto,Nombre,PrecioSabor")] Sabor sabor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sabor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Extras");
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "ProductoId", "Nombre", sabor.IdProducto);
            return View(sabor);
        }

        // GET: Sabores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sabor = await _context.Sabors.FindAsync(id);
            if (sabor == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "ProductoId", "Nombre", sabor.IdProducto);
            return View(sabor);
        }

        // POST: Sabores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaborId,IdProducto,Nombre,PrecioSabor")] Sabor sabor)
        {
            if (id != sabor.SaborId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sabor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaborExists(sabor.SaborId))
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
            ViewData["IdProducto"] = new SelectList(_context.Productos, "ProductoId", "Nombre", sabor.IdProducto);
            return View(sabor);
        }

        // GET: Sabores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sabor = await _context.Sabors
                .Include(s => s.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.SaborId == id);
            if (sabor == null)
            {
                return NotFound();
            }

            return View(sabor);
        }

        // POST: Sabores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sabor = await _context.Sabors.FindAsync(id);
            if (sabor != null)
            {
                _context.Sabors.Remove(sabor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaborExists(int id)
        {
            return _context.Sabors.Any(e => e.SaborId == id);
        }
    }
}
