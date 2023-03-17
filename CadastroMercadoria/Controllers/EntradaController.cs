using CadastroMercadoriaBiblioteca.Data;
using CadastroMercadoriaBiblioteca.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadastroMercadoria.Controllers
{
    public class EntradaController : Controller
    {
        private readonly MercadoriaDbContext _context;

        public EntradaController(MercadoriaDbContext context)
        {
            _context = context;
        }

        // GET: EntradaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: EntradaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EntradaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EntradaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarEntradaMercadoriaNova
            ([Bind("Id,Nome,NumeroRegistro,Quantidade,Fabricante,TipoDescricao")] Mercadoria mercadoria)
        {
            TimeZoneInfo brTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime dataHoraLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brTimeZone);

            if (ModelState.IsValid)
            {
                var entradas = new Entrada
                {
                    Quantidade = mercadoria.Quantidade,
                    DataHora = dataHoraLocal,
                    Local = "Brazil",
                };

                entradas.Mercadoria = mercadoria;
                mercadoria.Ativo = true;

                _context.Add(entradas);
                _context.Entradas.Add(entradas);

                return RedirectToAction(nameof(Index));
            }

            return View(mercadoria);
        }

        // POST: EntradaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarEntradaMercadoriaExistente
            ([Bind("Id,Nome,NumeroRegistro,Quantidade,Fabricante,TipoDescricao")] Mercadoria mercadoria)
        {
            TimeZoneInfo brTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime dataHoraLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brTimeZone);

            if (ModelState.IsValid)
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

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(mercadoria);
        }

        // GET: EntradaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EntradaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EntradaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EntradaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
