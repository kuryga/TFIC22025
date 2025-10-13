using System; 
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using SessionContext = DAL.Seguridad.SessionContext;

namespace DAL.Audit
{
    public sealed class BitacoraDAL
    {
        private static BitacoraDAL _instance;
        private BitacoraDAL() { }
        public static BitacoraDAL GetInstance()
        {
            if (_instance == null) _instance = new BitacoraDAL();
            return _instance;
        }

        private static readonly DalToolkit db = new DalToolkit();

        private const string TblBitacora = "dbo.Bitacora";
        private const string PkBitacora = "idRegistro";

        private static bool _isListingBitacora;

        public List<BE.Audit.Bitacora> GetBitacoraList(DateTime? desde, DateTime? hasta, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 30;

            int start = ((page - 1) * pageSize) + 1;
            int end = page * pageSize;

            var items = new List<BE.Audit.Bitacora>();

            string sql = @"
WITH CTE AS (
    SELECT
        b." + PkBitacora + @",
        b.fecha,
        b.criticidad,
        b.accion,
        b.mensaje,
        b.idEjecutor,
        b.UsuarioEjecutor,
        ROW_NUMBER() OVER (ORDER BY b.fecha DESC, b." + PkBitacora + @" DESC) AS rn
    FROM " + TblBitacora + @" b
    WHERE (@desde IS NULL OR CAST(b.fecha AS date) >= @desde)
      AND (@hasta IS NULL OR CAST(b.fecha AS date) <= @hasta)
)
SELECT
    " + PkBitacora + @" AS idRegistro,
    fecha,
    criticidad,
    accion,
    mensaje,
    idEjecutor,
    UsuarioEjecutor
FROM CTE
WHERE rn BETWEEN @start AND @end
ORDER BY rn;";

            using (var conn = new SqlConnection(DalToolkit.connectionString))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
            {
                cmd.Parameters.Add("@desde", SqlDbType.Date).Value =
                    desde.HasValue ? (object)desde.Value.Date : DBNull.Value;
                cmd.Parameters.Add("@hasta", SqlDbType.Date).Value =
                    hasta.HasValue ? (object)hasta.Value.Date : DBNull.Value;
                cmd.Parameters.Add("@start", SqlDbType.Int).Value = start;
                cmd.Parameters.Add("@end", SqlDbType.Int).Value = end;

                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    int oId = rdr.GetOrdinal("idRegistro");
                    int oFecha = rdr.GetOrdinal("fecha");
                    int oCrit = rdr.GetOrdinal("criticidad");
                    int oAcc = rdr.GetOrdinal("accion");
                    int oMsg = rdr.GetOrdinal("mensaje");
                    int oIdExec = rdr.GetOrdinal("idEjecutor");
                    int oUsr = rdr.GetOrdinal("UsuarioEjecutor");

                    while (rdr.Read())
                    {
                        var crit = BE.Audit.Criticidad.C5;
                        if (!rdr.IsDBNull(oCrit))
                        {
                            var s = rdr.GetString(oCrit);
                            if (Enum.TryParse(s, true, out BE.Audit.Criticidad tmp))
                                crit = tmp;
                        }

                        items.Add(new BE.Audit.Bitacora
                        {
                            IdRegistro = rdr.GetInt32(oId),
                            Fecha = rdr.GetDateTime(oFecha),
                            Criticidad = crit,
                            Accion = rdr.IsDBNull(oAcc) ? null : rdr.GetString(oAcc),
                            Mensaje = rdr.IsDBNull(oMsg) ? null : rdr.GetString(oMsg),
                            IdEjecutor = rdr.IsDBNull(oIdExec) ? (int?)null : rdr.GetInt32(oIdExec),
                            UsuarioEjecutor = rdr.IsDBNull(oUsr) ? null : rdr.GetString(oUsr),
                        });
                    }
                }
                conn.Close();
            }

            if (!_isListingBitacora)
            {
                try
                {
                    _isListingBitacora = true;

                    string msg =
                        $"Listado de bitácora (página={page}, tamaño={pageSize}, " +
                        $"desde={(desde.HasValue ? desde.Value.ToString("yyyy-MM-dd") : "-")}, " +
                        $"hasta={(hasta.HasValue ? hasta.Value.ToString("yyyy-MM-dd") : "-")}).";

                    Log(BE.Audit.AuditEvents.ConsultaBitacora, msg);
                }
                finally
                {
                    _isListingBitacora = false;
                }
            }

            return items;
        }

        public int Log(string accion, string mensaje, string criticidad)
        {
            if (accion == null) accion = string.Empty;
            if (mensaje == null) mensaje = string.Empty;
            if (criticidad == null) criticidad = string.Empty;

            var ctx = SessionContext.Current;
            int? uid = (ctx != null) ? ctx.UsuarioId : (int?)null;
            string uname = (ctx != null && !string.IsNullOrEmpty(ctx.UsuarioEmail))
                           ? ctx.UsuarioEmail
                           : "Usuario deslogeado";

            const string sql = @"
INSERT INTO " + TblBitacora + @"
( fecha, criticidad, accion, mensaje, idEjecutor, UsuarioEjecutor, DVH )
VALUES ( @fecha, @criticidad, @accion, @mensaje, @idEjecutor, @usuarioEjecutor, @DVH );
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId;

            using (var conn = new SqlConnection(DalToolkit.connectionString))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
            {
                cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@criticidad", SqlDbType.VarChar, 32).Value = criticidad;
                cmd.Parameters.Add("@accion", SqlDbType.VarChar, 128).Value = accion;
                cmd.Parameters.Add("@mensaje", SqlDbType.VarChar, 4000).Value = mensaje;

                if (uid.HasValue)
                    cmd.Parameters.Add("@idEjecutor", SqlDbType.Int).Value = uid.Value;
                else
                    cmd.Parameters.Add("@idEjecutor", SqlDbType.Int).Value = DBNull.Value;

                cmd.Parameters.Add("@usuarioEjecutor", SqlDbType.VarChar, 150).Value =
                    (object)uname ?? DBNull.Value;

                cmd.Parameters.Add("@DVH", SqlDbType.VarChar, 256).Value = string.Empty;

                conn.Open();
                newId = cmd.ExecuteScalar();
                conn.Close();
            }

            int newIdInt = (newId != null && newId != DBNull.Value) ? Convert.ToInt32(newId) : 0;
            if (newIdInt > 0)
                db.RefreshRowDvAndTableDvv(TblBitacora, PkBitacora, newIdInt, true);

            return newIdInt;
        }

        public int Log(string accion, string mensaje)
        {
            var crit = BE.Audit.AuditEvents.GetCriticidad(accion).ToString();
            return Log(accion, mensaje ?? string.Empty, crit);
        }
    }
}
