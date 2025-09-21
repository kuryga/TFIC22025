using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Bitacora
    {
        public int IdRegistro { get; set; }
        public DateTime Fecha { get; set; }
        public Criticidad Criticidad { get; set; } = Criticidad.C5;
        public string Mensaje { get; set; }
    }
}
