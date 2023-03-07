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
using System.Drawing.Printing;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;

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
            var mercadorias = await _context.Mercadorias
                .Where(m => m.Ativo) // filtro para pegar apenas as mercadorias ativas
                .ToListAsync();

            return View(mercadorias);
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
                .GroupBy(e => new { e.Mercadoria.Nome, Mes = e.DataHora.Month })
                .Select(g => new { Mercadoria = g.Key.Nome, Mes = g.Key.Mes, Quantidade = g.Sum(e => e.Quantidade) })
                .ToList();

            var saidas = _context.Saidas
                .Include(s => s.Mercadoria)
                .GroupBy(s => new { s.Mercadoria.Nome, Mes = s.DataHora.Month })
                .Select(g => new { Mercadoria = g.Key.Nome, Mes = g.Key.Mes, Quantidade = g.Sum(s => s.Quantidade) })
                .ToList();

            var data = new List<ChartDataViewModel>();
            foreach (var entrada in entradas)
            {
                var item = data.FirstOrDefault(d => d.Label == entrada.Mercadoria);
                if (item == null)
                {
                    item = new ChartDataViewModel();
                    item.Label = entrada.Mercadoria;
                    data.Add(item);
                }

                item.Entradas[entrada.Mes - 1] = entrada.Quantidade;
            }

            foreach (var saida in saidas)
            {
                var item = data.FirstOrDefault(d => d.Label == saida.Mercadoria);
                if (item == null)
                {
                    item = new ChartDataViewModel();
                    item.Label = saida.Mercadoria;
                    data.Add(item);
                }
                item.Saidas[saida.Mes - 1] = saida.Quantidade;
            }

            return Ok(data);
        }




        // GET: Estoque/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult ExportarRelatorioMensal()
        {
            var entradas = _context.Entradas.Include(e => e.Mercadoria).ToList();
            var saidas = _context.Saidas.Include(s => s.Mercadoria).ToList();

            var pdfDoc = new Document(PageSize.A4, 50, 50, 25, 25);
            var ms = new MemoryStream();
            var writer = PdfWriter.GetInstance(pdfDoc, ms);

            pdfDoc.Open();

            // Cria um parágrafo para o título do relatório
            var titulo = new Paragraph("Relatório Mensal de Entradas e Saídas");
            titulo.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(titulo);

            // Adiciona uma tabela com as entradas e saídas de cada mercadoria
            var table = new PdfPTable(5);
            table.WidthPercentage = 100;

            table.AddCell(new PdfPCell(new Phrase("Mercadoria")) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Número de registro")) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Quantidade de Entradas")) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Quantidade de Saídas")) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Saldo")) { HorizontalAlignment = Element.ALIGN_CENTER });

            var mercadorias = entradas.Select(e => e.Mercadoria).Union(saidas.Select(s => s.Mercadoria)).Distinct();

            foreach (var mercadoria in mercadorias)
            {
                var entradaTotal = entradas.Where(e => e.Mercadoria == mercadoria).Sum(e => e.Quantidade);
                var saidaTotal = saidas.Where(s => s.Mercadoria == mercadoria).Sum(s => s.Quantidade);
                var saldo = entradaTotal - saidaTotal;

                table.AddCell(new PdfPCell(new Phrase(mercadoria.Nome)));
                table.AddCell(new PdfPCell(new Phrase(mercadoria.NumeroRegistro.ToString())) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase(entradaTotal.ToString())) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase(saidaTotal.ToString())) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase(saldo.ToString())) { HorizontalAlignment = Element.ALIGN_CENTER });
            }

            pdfDoc.Add(table);

            pdfDoc.Close();

            var nomeArquivo = "RelatorioMensal.pdf";
            return File(ms.ToArray(), "application/pdf", nomeArquivo);
        }

        // POST: Estoque/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,NumeroRegistro,Quantidade,Fabricante,TipoDescricao")] Mercadoria mercadoria)
        {

            if (ModelState.IsValid)
            {
                TimeZoneInfo brTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                DateTime dataHoraLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brTimeZone);

                var mercadoriaExiste = await _context.Mercadorias.Where(m => m.NumeroRegistro == mercadoria.NumeroRegistro).ToListAsync();

                if (mercadoriaExiste.Count > 0)
                {
                    var entradas = new Entrada
                    {
                        Quantidade = mercadoria.Quantidade,
                        DataHora = dataHoraLocal,
                        Local = "Brazil",
                        MercadoriaId = mercadoriaExiste[0].Id,
                    };

                    _context.Add(entradas);
                    _context.Entradas.Add(entradas);

                    if (!MercadoriaRegistroExists(mercadoria.NumeroRegistro))
                    {
                        entradas.Mercadoria = mercadoria;
                        mercadoria.Ativo = true;
                        _context.Add(mercadoria);
                    }
                    else
                    {
                        mercadoria.Ativo = true;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                else
                {
                    var entradas = new Entrada
                    {
                        Quantidade = mercadoria.Quantidade,
                        DataHora = dataHoraLocal,
                        Local = "Brazil",
                        MercadoriaId = mercadoria.Id,
                    };

                    _context.Add(entradas);
                    _context.Entradas.Add(entradas);

                    if (!MercadoriaRegistroExists(mercadoria.NumeroRegistro))
                    {
                        entradas.Mercadoria = mercadoria;
                        mercadoria.Ativo = true;
                        _context.Add(mercadoria);
                    }
                    else
                    {
                        mercadoria.Ativo = true;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
               
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
                TimeZoneInfo brTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                DateTime dataHoraLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brTimeZone);

                var saida = new Saida
                {
                    Quantidade = mercadoria.Quantidade,
                    DataHora = dataHoraLocal,
                    Local = "Brazil",
                    MercadoriaId = mercadoria.Id,
                    Mercadoria = mercadoria
                };

                mercadoria.Ativo = false;
                _context.Mercadorias.Update(mercadoria);

                _context.Add(saida);
                _context.Saidas.Add(saida);
                await _context.SaveChangesAsync();
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
