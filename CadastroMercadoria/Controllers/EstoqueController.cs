using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CadastroMercadoriaBiblioteca.Data;
using CadastroMercadoriaBiblioteca.Models;
using System.Data;
using System.Globalization;

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

        public IActionResult GetChartData()
        {
            var entradas = _context.Entradas
                .Include(e => e.Mercadoria)
                .GroupBy(e => new { e.Mercadoria.Nome })
                .Select(g => new { Mercadoria = g.Key.Nome, Quantidade = g.Sum(e => e.Quantidade) })
                .ToList();

            var saidas = _context.Saidas
                .Include(s => s.Mercadoria)
                .GroupBy(s => new { s.Mercadoria.Nome })
                .Select(g => new { Mercadoria = g.Key.Nome, Quantidade = g.Sum(s => s.Quantidade) })
                .ToList();

            var data = new List<ChartDataViewModel>();
            foreach (var entrada in entradas)
            {
                var item = new ChartDataViewModel();
                item.Label = entrada.Mercadoria;
                item.Entrada = entrada.Quantidade;
                data.Add(item);
            }

            foreach (var saida in saidas)
            {
                var item = data.FirstOrDefault(d => d.Label == saida.Mercadoria);
                if (item != null)
                {
                    item.Saida = saida.Quantidade;
                }
                else
                {
                    item = new ChartDataViewModel();
                    item.Label = saida.Mercadoria;
                    item.Saida = saida.Quantidade;
                    data.Add(item);
                }
            }

            return Ok(data);
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
                DateTime dataHoraLocal = DateTime.Now;

                var entradas = new Entrada
                {
                    Quantidade = 1,
                    DataHora = TimeZoneInfo.ConvertTimeToUtc(dataHoraLocal),
                    Local = "Brazil",
                    MercadoriaId = mercadoria.Id,
                };

                entradas.Mercadoria = mercadoria;

                _context.Add(entradas);

                if (MercadoriaRegistroExists(mercadoria.NumeroRegistro))
                {
                    _context.Add(mercadoria);
                }         

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
                DateTime dataHoraLocal = DateTime.Now;         

                var saida = new Saida
                {
                    Quantidade = 1,
                    DataHora = TimeZoneInfo.ConvertTimeToUtc(dataHoraLocal),
                    Local = "Brazil",
                    MercadoriaId = mercadoria.Id,
                };

                saida.Mercadoria = mercadoria;

                _context.Saidas.Add(saida);                    
                //_context.Mercadorias.Remove(mercadoria);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MercadoriaExists(int id)
        {
          return _context.Mercadorias.Any(e => e.Id == id);
        }

        private bool MercadoriaRegistroExists(int numRegistro)
        {
            return _context.Mercadorias.Any(e => e.NumeroRegistro == numRegistro);
        }


    }
}
