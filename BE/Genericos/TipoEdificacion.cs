using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE {
    public class TipoEdificacion {
        public int IdTipoEdificacion { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool Deshabilitado { get; set; }
    }
}
