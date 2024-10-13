using ListaDeTarefasSimples.Models;

namespace ListaDeTarefasSimples.ViewModels
{
    public class HomeControllerViewModel
    {
        public IEnumerable<Tarefas> Lista { get; set; } = new List<Tarefas>();  

    }
}
