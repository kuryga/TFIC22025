using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Genericos
{
    public class IdiomaDAL
    {
        private static IdiomaDAL instance;
        private IdiomaDAL() { }
        public static IdiomaDAL GetInstance() { if (instance == null) instance = new IdiomaDAL(); return instance; }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.Idioma";
        private const string idCol = "idIdioma";
        private const string publicCols = "idIdioma, nombre, codigoISO";

        public List<BE.Idioma> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";
            return db.QueryListAndLog<BE.Idioma>(
                sql,
                null,
                table, idCol,
                BE.Audit.AuditEvents.ConsultaIdiomas,
                "Listado de idiomas",
                false
            );
        }   

        public Dictionary<string, string> LoadDiccionarioTraducciones(int idIdioma)
        {
            const string sql = @"
SELECT  t.codigo, it.texto
FROM    dbo.Traduccion         AS t
JOIN    dbo.IdiomaTraduccion   AS it ON it.idTraduccion = t.idTraduccion
WHERE   it.idIdioma = @idIdioma;";

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var cn = new SqlConnection(DalToolkit.connectionString))
            using (var cmd = new SqlCommand(sql, cn) { CommandType = CommandType.Text })
            {
                cmd.Parameters.Add("@idIdioma", SqlDbType.Int).Value = idIdioma;
                cn.Open();
                using (var rd = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    while (rd.Read())
                    {
                        var codigo = rd.GetString(0);
                        var texto = rd.GetString(1);
                        dict[codigo] = texto;
                    }
                }
            }

            return dict;
        }

        public Dictionary<string, string> GetDiccionarioTraduccionesPorIso(string codigoIso)
        {
            if (string.IsNullOrWhiteSpace(codigoIso))
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            const string sqlId = "SELECT " + idCol + " FROM " + table + " WHERE codigoISO = @iso;";

            int? idIdioma = null;
            using (var cn = new SqlConnection(DalToolkit.connectionString))
            using (var cmd = new SqlCommand(sqlId, cn) { CommandType = CommandType.Text })
            {
                cmd.Parameters.Add("@iso", SqlDbType.VarChar, 10).Value = codigoIso;
                cn.Open();
                object o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                    idIdioma = Convert.ToInt32(o);
            }

            return idIdioma.HasValue
                ? LoadDiccionarioTraducciones(idIdioma.Value)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
