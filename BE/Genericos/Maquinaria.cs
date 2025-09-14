using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE{    
    public class Maquinaria {
        public int IdMaquinaria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal CostoPorHora { get; set; }
    }
}
