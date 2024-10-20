using ClosedXML.Excel;
using ListaDeTarefasSimples.Context;
using ListaDeTarefasSimples.Models;
using ListaDeTarefasSimples.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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
            else if (formato == "pdf")
            {
                var nomeArquivo = $"Tarefas.pdf";
                return GerarPdf(nomeArquivo, tarefas);
            }



            return RedirectToAction("Index");
        }
        private FileResult GerarPdf(string nomeArquivo, IEnumerable<Tarefas> tarefas)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = nomeArquivo;

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);

            double xPos = 20; // Margem esquerda
            double yPos = 20; // Margem superior
            double rowHeight = 20; // Altura de cada linha

            gfx.DrawString("Tarefa", font, XBrushes.Black, xPos, yPos); 
            gfx.DrawString("Data", font, XBrushes.Black, xPos + 200, yPos); // 200 unidades de offset
            gfx.DrawString("Status", font, XBrushes.Black, xPos + 400, yPos); // 400 unidades de offset
            yPos += rowHeight; // Move para a próxima linha

            foreach (var tarefa in tarefas)
            {
                gfx.DrawString(tarefa.Tarefa, font, XBrushes.Black, xPos, yPos);
                gfx.DrawString(tarefa.Data.ToString("dd/MM/yyyy"), font, XBrushes.Black, xPos + 200, yPos);
                gfx.DrawString(tarefa.Status ? "Concluída" : "Pendente", font, XBrushes.Black, xPos + 400, yPos);
                yPos += rowHeight; // Move para a próxima linha
            }

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, false);
                return File(stream.ToArray(),
                    "application/pdf",
                    nomeArquivo);
            }

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
