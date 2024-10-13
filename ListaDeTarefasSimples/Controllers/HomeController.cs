using ListaDeTarefasSimples.Context;
using ListaDeTarefasSimples.Models;
using ListaDeTarefasSimples.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
            var model = new HomeControllerViewModel
            {
                Lista = _context.Tarefas.ToList(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Criar(Tarefas tarefas)
        {
            if (ModelState.IsValid)
            {

                _context.Tarefas.Add(tarefas);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(tarefas);
        }
    }
}
