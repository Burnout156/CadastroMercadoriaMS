using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroMercadoriaBiblioteca.Models
{
    public class Entrada
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public int Quantidade { get; set; }
        public string Local { get; set; }
        public int MercadoriaId { get; set; }
        public virtual Mercadoria Mercadoria { get; set; }
    }
}
