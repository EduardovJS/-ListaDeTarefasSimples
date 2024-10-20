using ClosedXML.Excel;
using ListaDeTarefasSimples.Context;
using ListaDeTarefasSimples.Models;
using ListaDeTarefasSimples.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace ListaDeTarefasSimples.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.DataAtual = DateTime.Now;
            var model = new HomeControllerViewModel
            {
                Lista = _context.Tarefas.ToList(),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Exportar(string formato)
        {
            var tarefas = await _context.Tarefas.ToListAsync();

            if (formato == "xlsx")
            {
                var nomeArquivo = $"Tarefas.xlsx";
                return GerarExcel(nomeArquivo, tarefas);

            }
            return RedirectToAction("Index");
        }

        private FileResult GerarExcel(string nomeArquivo, IEnumerable<Tarefas> tarefas)
        {
            DataTable datatable = new DataTable("Tarefas");
            datatable.Columns.AddRange(new DataColumn[] {
                new DataColumn("Tarefa"),
                new DataColumn("Data"),new DataColumn("Status"),
                });

            foreach (var tarefa in tarefas)
            {
                datatable.Rows.Add(tarefa.Tarefa,
                    tarefa.Data, tarefa.Status);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(datatable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nomeArquivo);
                }
            }
        }

        [HttpPost("Criar")]
        public IActionResult Criar(Tarefas tarefas)
        {


            if (ModelState.IsValid)
            {

                tarefas.Data = DateTime.Now;
                _context.Tarefas.Add(tarefas);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(tarefas);
        }

        [HttpPost("Remover")]
        public IActionResult Remover(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa != null)
            {
                _context.Tarefas.Remove(tarefa);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [HttpPost("Editar")]
        public IActionResult Editar(Tarefas tarefas)
        {
            if (ModelState.IsValid)
            {

                _context.Tarefas.Update(tarefas);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(tarefas);
        }

        [HttpPost("Concluida")]
        public IActionResult Concluida(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa != null)
            {
                tarefa.Status = !tarefa.Status;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }




    }
}
