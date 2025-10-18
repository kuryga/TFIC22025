using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Seguridad
{
    public sealed class SessionContext
    {
        private static readonly SessionContext _current = new SessionContext();
        public static SessionContext Current { get { return _current; } }

        public int? UsuarioId { get; set; }
        public string UsuarioEmail { get; set; }
        public string NombreCompleto { get; set; }

        public HashSet<string> Patentes { get; private set; }

        private SessionContext()
        {
            this.ClearSession();
        }

        public void ClearSession()
        {
            UsuarioId = null;
            UsuarioEmail = "Usuario deslogeado";
            NombreCompleto = "Usuario deslogeado";
            Patentes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public void SetPatentes(IEnumerable<BE.Patente> patentes)
        {
            Patentes = patentes == null
                ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(
                    patentes
                        .Select(p => p?.NombrePatente ?? string.Empty)
                        .Where(s => !string.IsNullOrWhiteSpace(s)),
                    StringComparer.OrdinalIgnoreCase
                  );
        }

        public bool TienePatente(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return false;
            return Patentes.Contains(codigo);
        }

        public bool TieneAlguna(params string[] codigos)
        {
            if (codigos == null || codigos.Length == 0) return false;
            return codigos.Any(c => TienePatente(c));
        }
    }
}
