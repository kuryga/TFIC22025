using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Patente
    {
            public int IdPatente { get; set; }
            public string NombrePatente { get; set; } = string.Empty; // Unique
            public string Descripcion { get; set; }
    }
}
