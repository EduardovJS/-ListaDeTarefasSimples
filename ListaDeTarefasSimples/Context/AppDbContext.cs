using ListaDeTarefasSimples.Models;
using Microsoft.EntityFrameworkCore;

namespace ListaDeTarefasSimples.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tarefas> Tarefas { get; set; }

    }
}
