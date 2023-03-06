using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroMercadoriaBiblioteca.Models
{
    public class Mercadoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da mercadoria é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome da mercadoria deve ter no máximo 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O número de registro é obrigatório.")]
        public int NumeroRegistro { get; set; }

        [Required(ErrorMessage = "O fabricante é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome do fabricante deve ter no máximo 50 caracteres.")]
        public string Fabricante { get; set; }

        [Required(ErrorMessage = "O tipo da descrição é obrigatório.")]
        [StringLength(50, ErrorMessage = "O tipo da descrição deve ter no máximo 50 caracteres.")]
        public string TipoDescricao { get; set; }

        public bool Ativo { get; set; }
    }
}
