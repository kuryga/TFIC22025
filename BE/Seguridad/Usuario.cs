using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string ApellidoUsuario { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string TelefonoContacto { get; set; }
        public string DireccionUsuario { get; set; }
        public string NumeroDocumento { get; set; }
        public bool Bloqueado { get; set; }
    }
}
