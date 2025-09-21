using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Familia
    {
        public int IdFamilia { get; set; }
        public string NombreFamilia { get; set; } = string.Empty; // Unique
        public string Descripcion { get; set; }
    }
}
