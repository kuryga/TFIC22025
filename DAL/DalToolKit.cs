using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

using SessionContext = DAL.Seguridad.SessionContext;
public sealed class DalToolkit
{
    // Connection string global
    public const string connectionString =
        @"Server=.;Database=UrbanSoft;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

    private const string dvvTable = "[dbo].[DigitoVerificadorVertical]";
    private const string dvvColTableName = "tabla";
    private const string dvvColValue = "valorDVV";
    private const string dvvColDvh = "DVH";

    // ===== Bitácora =====
    private const string TblBitacora = "dbo.Bitacora";
    private const string PkBitacora = "idRegistro";


    /// <summary>
    /// Ejecuta INSERT/UPDATE/DELETE, recalcula DVH/DVV y registra en Bitácora.
    /// Loguea fallos con acción C1 y detalle (tipo de llamado + tabla + mensaje).
    /// </summary>
    public int ExecuteNonQueryAndLog(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName, string pkName,
        string accion, string mensaje)
    {
        try
        {
            int rows = ExecuteNonQuery(sql, bindParams);
            RecalculateTableDvsFromSelectAll(tableName, pkName);
            LogAuto(accion, mensaje);
            return rows;
        }
        catch (Exception ex)
        {
            LogDbFailure("ExecuteNonQuery", tableName, ex);
            throw;
        }
    }

    /// <summary>
    /// Ejecuta INSERT que retorna SCOPE_IDENTITY (u otro scalar), recalcula DVH/DVV y registra en Bitácora.
    /// Loguea fallos con acción C1 y detalle (tipo de llamado + tabla + mensaje).
    /// </summary>
    public object ExecuteScalarAndLog(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName, string pkName,
        string accion, string mensaje)
    {
        try
        {
            object obj = ExecuteScalar(sql, bindParams);
            RecalculateTableDvsFromSelectAll(tableName, pkName);
            LogAuto(accion, mensaje);
            return obj;
        }
        catch (Exception ex)
        {
            LogDbFailure("ExecuteScalar", tableName, ex);
            throw;
        }
    }

    /// <summary>
    /// Ejecuta SELECT (lista), recalcula DVH/DVV y registra en Bitácora (lectura).
    /// Loguea fallos con acción C1 y detalle (tipo de llamado + tabla + mensaje).
    /// </summary>
    public List<T> QueryListAndLog<T>(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn,
        string accion, string mensaje) where T : new()
    {
        try
        {
            var rows = ExecuteList<T>(sql, bindParams);
            RecalculateTableDvsFromSelectAll(tableName, idColumn);
            LogAuto(accion, mensaje);
            return rows;
        }
        catch (Exception ex)
        {
            LogDbFailure("QueryList", tableName, ex);
            throw;
        }
    }

    /// <summary>
    /// Ejecuta SELECT (single or default), recalcula DVH/DVV y registra en Bitácora (lectura).
    /// </summary>
    public T QuerySingleOrDefaultAndLog<T>(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn,
        string accion, string mensaje) where T : new()
    {
        var list = QueryListAndLog<T>(sql, bindParams, tableName, idColumn, accion, mensaje);
        return list.Count > 0 ? list[0] : default(T);
    }

    private int LogAuto(string accion, string mensaje)
    {
        var crit = BE.Audit.AuditEvents.GetCriticidad(accion).ToString();
        return LogBitacora(accion, mensaje ?? string.Empty, crit);
    }

    private void LogDbFailure(string callType, string tableName, Exception ex)
    {
        string accion = BE.Audit.AuditEvents.FalloConexionBD;
        string tablaKey = DvvKey(tableName); // "usuario" en vez de "dbo.Usuario"
        string detalle = (ex != null) ? ex.Message : "DB error";
        string msg = (callType ?? "DBCall") + " sobre " + (tablaKey ?? "?") + ": " + detalle;
        LogBitacora(accion, msg, BE.Audit.AuditEvents.GetCriticidad(accion).ToString());
    }

    private List<T> ExecuteList<T>(string sql, Action<SqlCommand> bind) where T : new()
    {
        var result = new List<T>();
        var conn = new SqlConnection(connectionString);
        var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
        if (bind != null) bind(cmd);

        try
        {
            conn.Open();
            var reader = cmd.ExecuteReader();
            result = DbMapper.MapToList<T>(reader);
            conn.Close();
        }
        catch
        {
            if (conn.State == ConnectionState.Open) conn.Close();
            throw;
        }
        return result;
    }

    private int ExecuteNonQuery(string sql, Action<SqlCommand> bindParams)
    {
        var conn = new SqlConnection(connectionString);
        var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
        if (bindParams != null) bindParams(cmd);

        try
        {
            conn.Open();
            var rows = cmd.ExecuteNonQuery();
            conn.Close();
            return rows;
        }
        catch
        {
            if (conn.State == ConnectionState.Open) conn.Close();
            throw;
        }
    }

    private object ExecuteScalar(string sql, Action<SqlCommand> bindParams)
    {
        var conn = new SqlConnection(connectionString);
        var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
        if (bindParams != null) bindParams(cmd);

        try
        {
            conn.Open();
            var obj = cmd.ExecuteScalar();
            conn.Close();
            return obj;
        }
        catch
        {
            if (conn.State == ConnectionState.Open) conn.Close();
            throw;
        }
    }

    public void RecalculateTableDvsFromSelectAll(string tableName, string idColumn)
    {
        var conn = new SqlConnection(connectionString);
        var cmd = new SqlCommand("SELECT * FROM " + tableName + ";", conn) { CommandType = CommandType.Text };

        var dvhs = new List<string>();

        try
        {
            conn.Open();
            var rdr = cmd.ExecuteReader();

            if (!rdr.HasRows)
            {
                conn.Close();
                UpsertDvvWithDvh(tableName, string.Empty);
                return;
            }

            var cols = new List<Tuple<string, int>>();
            int idOrdinal = -1;

            for (int i = 0; i < rdr.FieldCount; i++)
            {
                var colName = rdr.GetName(i);
                if (string.Equals(colName, "DVH", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (string.Equals(colName, idColumn, StringComparison.OrdinalIgnoreCase))
                    idOrdinal = i;

                cols.Add(Tuple.Create(colName, i));
            }

            if (idOrdinal < 0)
                throw new InvalidOperationException("No se encontró la columna Id '" + idColumn + "' en " + tableName + ".");

            cols.Sort((a, b) => string.Compare(a.Item1, b.Item1, StringComparison.Ordinal));

            while (rdr.Read())
            {
                var sb = new StringBuilder();

                for (int c = 0; c < cols.Count; c++)
                {
                    int ord = cols[c].Item2;
                    object v = rdr.IsDBNull(ord) ? null : rdr.GetValue(ord);

                    if (v == null)
                    {
                        // nada
                    }
                    else if (v is byte[])
                    {
                        var bytes = (byte[])v;
                        for (int k = 0; k < bytes.Length; k++)
                            sb.Append(bytes[k].ToString("x2"));
                    }
                    else
                    {
                        sb.Append(Convert.ToString(v, CultureInfo.InvariantCulture));
                    }
                }

                string dvh = SimpleDv(sb.ToString());
                dvhs.Add(dvh);

                object idValue = rdr.GetValue(idOrdinal);
                UpdateRowDvh(tableName, idColumn, idValue, dvh);
            }

            conn.Close();

            string dvv = ComputeDvv(dvhs);
            UpsertDvvWithDvh(tableName, dvv);
        }
        catch (Exception ex)
        {
            if (conn.State == ConnectionState.Open) conn.Close();
            LogDbFailure("RecalculateDV", tableName, ex);
            throw;
        }
    }

    private void UpdateRowDvh(string table, string idColumn, object idValue, string dvh)
    {
        var sql = "UPDATE " + table + " SET DVH = @dvh WHERE " + idColumn + " = @id;";
        var conn = new SqlConnection(connectionString);
        var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@dvh", SqlDbType.VarChar, 256).Value = (object)dvh ?? string.Empty;
        cmd.Parameters.Add("@id", SqlDbType.Variant).Value = idValue ?? DBNull.Value;

        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        catch (Exception ex)
        {
            if (conn.State == ConnectionState.Open) conn.Close();
            LogDbFailure("UpdateRowDvh", table, ex);
            throw;
        }
    }

    private void UpsertDvvWithDvh(string tableName, string dvv)
    {
        var key = DvvKey(tableName); // por ejemplo "usuario"
        var conn = new SqlConnection(connectionString);

        try
        {
            conn.Open();

            var upd = new SqlCommand(@"
UPDATE " + dvvTable + @"
   SET " + dvvColValue + @" = @dvv
 WHERE " + dvvColTableName + @" = @tabla;", conn);

            upd.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
            upd.Parameters.Add("@dvv", SqlDbType.VarChar, 256).Value = dvv ?? string.Empty;

            int rows = upd.ExecuteNonQuery();
            if (rows == 0)
            {
                var ins = new SqlCommand(@"
INSERT INTO " + dvvTable + @" (" + dvvColTableName + "," + dvvColValue + @")
VALUES (@tabla, @dvv);", conn);
                ins.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
                ins.Parameters.Add("@dvv", SqlDbType.VarChar, 256).Value = dvv ?? string.Empty;
                ins.ExecuteNonQuery();
            }

            var dvh = SimpleDv((key ?? "") + (dvv ?? ""));
            var updDvh = new SqlCommand(@"
UPDATE " + dvvTable + @"
   SET " + dvvColDvh + @" = @dvh
 WHERE " + dvvColTableName + @" = @tabla;", conn);

            updDvh.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
            updDvh.Parameters.Add("@dvh", SqlDbType.VarChar, 256).Value = dvh ?? string.Empty;
            updDvh.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            LogDbFailure("UpsertDVV", dvvTable, ex);
            throw;
        }
        finally
        {
            if (conn.State == ConnectionState.Open) conn.Close();
        }
    }

    private static string DvvKey(string tableName)
    {
        if (string.IsNullOrEmpty(tableName)) return string.Empty;
        int dot = tableName.LastIndexOf('.');
        var name = dot >= 0 ? tableName.Substring(dot + 1) : tableName;
        return name.ToLowerInvariant(); // "usuario"
    }

    private static string ComputeDvv(List<string> dvhs)
    {
        string acc = string.Empty;
        for (int i = 0; i < dvhs.Count; i++)
            acc = SimpleDv(acc + (dvhs[i] ?? string.Empty));
        return acc;
    }

    private static string SimpleDv(string s)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(s ?? string.Empty);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));
            return sb.ToString();
        }
    }

    private int LogBitacora(string accion, string mensaje, string criticidad)
    {
        if (accion == null) accion = string.Empty;
        if (mensaje == null) mensaje = string.Empty;
        if (criticidad == null) criticidad = string.Empty;

        var ctx = SessionContext.Current;
        int? uid = (ctx != null) ? ctx.UsuarioId : (int?)null;
        string uname = (ctx != null && !string.IsNullOrEmpty(ctx.UsuarioEmail))
                        ? ctx.UsuarioEmail
                        : "Usuario deslogeado";

        string sql = @"
INSERT INTO dbo.Bitacora
( fecha, criticidad, accion, mensaje, idEjecutor, usuarioEjecutor, DVH )
VALUES ( @fecha, @criticidad, @accion, @mensaje, @idEjecutor, @usuarioEjecutor, @DVH );
SELECT CAST(SCOPE_IDENTITY() AS int);";

        object newId = ExecuteScalar(sql,
            cmd =>
            {
                cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.UtcNow;
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
            });

        RecalculateTableDvsFromSelectAll(TblBitacora, PkBitacora);

        return (newId != null && newId != DBNull.Value) ? Convert.ToInt32(newId) : 0;
    }

}
