using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CadastroMercadoriaBiblioteca.Data;
using CadastroMercadoriaBiblioteca.Models;

namespace CadastroMercadoria.Controllers
{
    public class EstoqueController : Controller
    {
        private readonly MercadoriaDbContext _context;

        public EstoqueController(MercadoriaDbContext context)
        {
            _context = context;
        }

        // GET: Estoque
        public async Task<IActionResult> Index()
        {
              return View(await _context.Mercadorias.ToListAsync());
        }

        // GET: Estoque/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Mercadorias == null)
            {
                return NotFound();
            }

            var mercadoria = await _context.Mercadorias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mercadoria == null)
            {
                return NotFound();
            }

            return View(mercadoria);
        }

        // GET: Estoque/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estoque/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,NumeroRegistro,Fabricante,TipoDescricao")] Mercadoria mercadoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mercadoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mercadoria);
        }

        // GET: Estoque/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Mercadorias == null)
            {
                return NotFound();
            }

            var mercadoria = await _context.Mercadorias.FindAsync(id);
            if (mercadoria == null)
            {
                return NotFound();
            }
            return View(mercadoria);
        }

        // POST: Estoque/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,NumeroRegistro,Fabricante,TipoDescricao")] Mercadoria mercadoria)
        {
            if (id != mercadoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mercadoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MercadoriaExists(mercadoria.Id))
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
            return View(mercadoria);
        }

        // GET: Estoque/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Mercadorias == null)
            {
                return NotFound();
            }

            var mercadoria = await _context.Mercadorias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mercadoria == null)
            {
                return NotFound();
            }

            return View(mercadoria);
        }

        // POST: Estoque/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Mercadorias == null)
            {
                return Problem("Entity set 'MercadoriasDbContext.Mercadorias'  is null.");
            }
            var mercadoria = await _context.Mercadorias.FindAsync(id);
            if (mercadoria != null)
            {
                _context.Mercadorias.Remove(mercadoria);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MercadoriaExists(int id)
        {
          return _context.Mercadorias.Any(e => e.Id == id);
        }

        // Action para exibir a tela de cadastro de entrada
        /*public ActionResult CadastroEntrada()
        {
            ViewBag.Mercadorias = new SelectList(_context.Mercadorias, "Id", "Nome");
            return View();
        }

        // Action para processar o formulário de cadastro de entrada
        [HttpPost]
        public ActionResult CadastroEntrada(Entrada entrada)
        {
            _context.Entradas.Add(entrada);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Action para exibir a tela de cadastro de saída
        public ActionResult CadastroSaida()
        {
            ViewBag.Mercadorias = new SelectList(_context.Mercadorias, "Id", "Nome");
            return View();
        }

        // Action para processar o formulário de cadastro de saída
        [HttpPost]
        public ActionResult CadastroSaida(Saida saida)
        {
            _context.Saidas.Add(saida);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }*/
    }
}
