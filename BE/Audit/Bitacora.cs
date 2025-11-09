using System;

namespace BE.Audit
{
    public class Bitacora
    {
        public int IdRegistro { get; set; }
        public DateTime Fecha { get; set; }
        public string Accion { get; set; }
        public Criticidad Criticidad { get; set; } = Criticidad.C5;
        public string Mensaje { get; set; }
        public int? IdEjecutor { get; set; }
        public string UsuarioEjecutor { get; set; }
    }
}
