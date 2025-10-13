using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.Moneda>;

namespace DAL.Genericos
{
    public class MonedaDAL : DaoInterface
    {
        private static MonedaDAL instance;
        private MonedaDAL() { }
        public static MonedaDAL GetInstance() { if (instance == null) instance = new MonedaDAL(); return instance; }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.Moneda";
        private const string idCol = "idMoneda";
        private const string publicCols = "idMoneda, NombreMoneda, Simbolo, ValorCambio";

        public List<BE.Moneda> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";
            return db.QueryListAndLog<BE.Moneda>(
                sql,
                null,
                table, idCol,
                BE.Audit.AuditEvents.ConsultaMonedas,
                "Listado de monedas",
                false
            );
        }

        public void Create(BE.Moneda obj)
        {
            var sql = @"
INSERT INTO " + table + @" (NombreMoneda, Simbolo, ValorCambio)
VALUES (@NombreMoneda, @Simbolo, @ValorCambio);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@NombreMoneda", SqlDbType.VarChar, 100).Value =
                        (object)obj.NombreMoneda ?? System.DBNull.Value;

                    cmd.Parameters.Add("@Simbolo", SqlDbType.VarChar, 10).Value =
                        (object)obj.Simbolo ?? System.DBNull.Value;

                    var pCambio = cmd.Parameters.Add("@ValorCambio", SqlDbType.Decimal);
                    pCambio.Precision = 18; pCambio.Scale = 4;
                    pCambio.Value = obj.ValorCambio;
                },
                table, idCol,
                BE.Audit.AuditEvents.AltaMoneda,
                "Alta de moneda: " + (obj.NombreMoneda ?? string.Empty),
                false
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdMoneda = System.Convert.ToInt32(newId);

            if (obj.IdMoneda > 0)
                db.RefreshRowDvAndTableDvv(table, idCol, obj.IdMoneda, false);
        }

        public void Update(BE.Moneda obj)
        {
            var sql = @"
UPDATE " + table + @"
   SET NombreMoneda = @NombreMoneda,
       Simbolo      = @Simbolo,
       ValorCambio  = @ValorCambio
 WHERE " + idCol + @" = @IdMoneda;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@IdMoneda", SqlDbType.Int).Value = obj.IdMoneda;

                    cmd.Parameters.Add("@NombreMoneda", SqlDbType.VarChar, 100).Value =
                        (object)obj.NombreMoneda ?? System.DBNull.Value;

                    cmd.Parameters.Add("@Simbolo", SqlDbType.VarChar, 10).Value =
                        (object)obj.Simbolo ?? System.DBNull.Value;

                    var pCambio = cmd.Parameters.Add("@ValorCambio", SqlDbType.Decimal);
                    pCambio.Precision = 18; pCambio.Scale = 4;
                    pCambio.Value = obj.ValorCambio;
                },
                table, idCol, obj.IdMoneda,
                BE.Audit.AuditEvents.ModificacionValorMoneda,
                "Modificación de moneda Id=" + obj.IdMoneda,
                true
            );
        }
    }
}
