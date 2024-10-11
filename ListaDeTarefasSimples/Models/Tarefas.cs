using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ListaDeTarefasSimples.Models
{
    [Table("Tarefas")]
    public class Tarefas
    {

        [Key]
        public int Id { get; set; }
        [StringLength(200, ErrorMessage = "O tamanho máximo é de 200 caracteres")]
        [Required(ErrorMessage = "Informe a tarefa")]
        [Display(Name = "Tarefa")]
        public string Tarefa { get; set; }
        public DateTime Data { get; set; }
        public bool Status { get; set; }
    }


}

