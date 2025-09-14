using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE {
    public class Moneda {
        public int IdMoneda { get; set; }
        public string NombreMoneda { get; set; } = string.Empty; // Unique
        public decimal ValorCambio { get; set; }
    }
}
