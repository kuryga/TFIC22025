using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Parametrizacion = BE.Params.Parametrizacion;

namespace DAL.Genericos
{
    public class ParametrizacionDAL
    {
        private static ParametrizacionDAL instance;
        private ParametrizacionDAL() { }

        public static ParametrizacionDAL GetInstance()
        {
            if (instance == null)
                instance = new ParametrizacionDAL();
            return instance;
        }

        private static readonly DalToolkit db = new DalToolkit();

        private const string TblParametrizacion = "dbo.Parametrizacion";
        private const string TblIdioma = "dbo.Idioma";

        public Parametrizacion GetParametrizacion()
        {
            var sql = @"
                SELECT p.nombreEmpresa, p.cuit, p.idIdioma, i.nombre, i.codigoISO
                FROM " + TblParametrizacion + @" p
                LEFT JOIN " + TblIdioma + " i ON p.idIdioma = i.idIdioma; ";

            var result = db.QuerySingleOrDefaultAndLog<Parametrizacion>(
                sql,
                null,
                TblParametrizacion, "idIdioma",
                BE.Audit.AuditEvents.ConsultaParametrizacion,
                "Obtener datos de parametrización"
            );

            return result;
        }
    }
}
