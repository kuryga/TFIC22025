using System;
using System.Collections.Generic;

namespace DAL.Genericos
{
    public static class TraduccionContext
    {
        private static Dictionary<string, string> _map =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static void CargarTraducciones()
        {

            int idioma = Seguridad.SessionContext.Current.parametrizacion.IdIdioma;     
            _map = IdiomaDAL.GetInstance().LoadDiccionarioTraducciones(idioma);
        }

        public static void CargarTraduccionesEspecificas(int idioma)
        {
            _map = IdiomaDAL.GetInstance().LoadDiccionarioTraducciones(idioma);
        }

        public static string Traducir(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return string.Empty;

            return _map.TryGetValue(codigo, out var texto) ? texto : codigo;
        }
    }
}
