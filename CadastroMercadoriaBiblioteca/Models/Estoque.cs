using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroMercadoriaBiblioteca.Models
{
    public class Estoque
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int NumeroRegistro { get; set; }
        public string Fabricante { get; set; }
        public string TipoDescricao { get; set; }
        public List<Entrada> Entradas { get; set; }
        public List<Saida> Saidas { get; set; }
        public List<string> Labels { get; set; }
    }
}
