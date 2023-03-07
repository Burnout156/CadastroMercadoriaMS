using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroMercadoriaBiblioteca.Models
{
    public class ChartDataViewModel
    {
        public string Label { get; set; }
        public int Entrada { get; set; }
        public int Saida { get; set; }
        public int[] Entradas { get; set; }
        public int[] Saidas { get; set; }

        public ChartDataViewModel() 
        {
            Entradas = new int[12];
            Saidas = new int[12];
        }
    }
}
