using System;

namespace BE
{
    public class Patente
    {
            public int IdPatente { get; set; }
            public string NombrePatente { get; set; } = string.Empty; // Unique
            public string Descripcion { get; set; }
    }
}
