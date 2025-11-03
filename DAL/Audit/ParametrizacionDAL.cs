using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Parametrizacion = BE.Params.Parametrizacion;

namespace DAL.Genericos
{
    public class ParametrizacionDAL
    {
        private static ParametrizacionDAL instance;
        private static readonly DalToolkit db = new DalToolkit();
        private static readonly int idEmpresa;

        private const string TblParametrizacion = "dbo.Parametrizacion";
        private const string TblIdioma = "dbo.Idioma";

        static ParametrizacionDAL()
        {
 
            int.TryParse(ConfigurationManager.AppSettings["IdEmpresa"], out idEmpresa);
            if (idEmpresa <= 0)
                idEmpresa = 1;
        }

        private ParametrizacionDAL() { }

        public static ParametrizacionDAL GetInstance()
        {
            if (instance == null)
                instance = new ParametrizacionDAL();
            return instance;
        }

        public Parametrizacion GetParametrizacion()
        {
            var sql = @"
                SELECT 
                    p.idEmpresa,
                    p.nombreEmpresa,
                    p.cuit,
                    p.idIdioma,
                    i.nombre,
                    i.codigoISO
                FROM " + TblParametrizacion + @" p
                LEFT JOIN " + TblIdioma + @" i ON p.idIdioma = i.idIdioma
                WHERE p.idEmpresa = @idEmpresa;";

            Action<SqlCommand> bind = cmd =>
            {
                cmd.Parameters.Add("@idEmpresa", SqlDbType.Int).Value = idEmpresa;
            };

            var result = db.QuerySingleOrDefaultAndLog<Parametrizacion>(
                sql,
                bind,
                TblParametrizacion,
                "idEmpresa",
                BE.Audit.AuditEvents.ConsultaParametrizacion,
                "Obtener datos de parametrización por empresa"
            );

            return result;
        }
    }
}
