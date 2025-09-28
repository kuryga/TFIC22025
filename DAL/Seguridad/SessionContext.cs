using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Seguridad
{
    public sealed class SessionContext
    {
        private static readonly SessionContext _current = new SessionContext();
        public static SessionContext Current { get { return _current; } }

        public int? UsuarioId { get; set; }
        public string UsuarioEmail { get; set; }

        private SessionContext()
        {
            // TODO: sacar este Mock cuando haga el login xd 
            UsuarioId = 1;
            UsuarioEmail = "Usuario deslogeado";

           // SessionContext.Current.UsuarioId = usuario.IdUsuario;
           // SessionContext.Current.UsuarioEmail = usuario.CorreoElectronico;
        }
    }
}
