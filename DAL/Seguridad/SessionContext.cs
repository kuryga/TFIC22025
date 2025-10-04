using System;

namespace DAL.Seguridad
{
    public sealed class SessionContext
    {
        private static readonly SessionContext _current = new SessionContext();
        public static SessionContext Current { get { return _current; } }

        public int? UsuarioId { get; set; }
        public string UsuarioEmail { get; set; }
        public string NombreCompleto { get; set; }

        private SessionContext()
        {
            // Mock
            UsuarioId = 1;
            UsuarioEmail = "Usuario deslogeado";
            NombreCompleto = "Usuario deslogeado";
        }
    }
}
