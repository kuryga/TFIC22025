using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Cotizacion
    {
        public int IdCotizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int IdTipoEdificacion { get; set; }
        public int IdMoneda { get; set; }
    }
}
