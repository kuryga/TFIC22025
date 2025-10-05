using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

public sealed class DalToolkit
{
    public const string connectionString =
        @"Server=.;Database=UrbanSoft;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

    private const string dvvTable = "[dbo].[DigitoVerificadorVertical]";
    private const string dvvColTableName = "tabla";
    private const string dvvColValue = "valorDVV";
    private const string dvvColDvh = "DVH";

    public int ExecuteNonQueryAndLog(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName, string pkName,
        string accion, string mensaje)
    {
        int rows = ExecuteNonQuery(sql, bindParams, "NONQUERY", tableName);
        RecalculateTableDvsFromSelectAll(tableName, pkName);
        DAL.Audit.BitacoraDAL.GetInstance().Log(accion, mensaje);
        return rows;
    }

    public object ExecuteScalarAndLog(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName, string pkName,
        string accion, string mensaje)
    {
        object obj = ExecuteScalar(sql, bindParams, "SCALAR", tableName);
        RecalculateTableDvsFromSelectAll(tableName, pkName);
        DAL.Audit.BitacoraDAL.GetInstance().Log(accion, mensaje);
        return obj;
    }

    public List<T> QueryListAndLog<T>(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn,
        string accion, string mensaje) where T : new()
    {
        var rows = ExecuteList<T>(sql, bindParams, "SELECT", tableName);
        RecalculateTableDvsFromSelectAll(tableName, idColumn);
        DAL.Audit.BitacoraDAL.GetInstance().Log(accion, mensaje);
        return rows;
    }

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

    private List<T> ExecuteList<T>(string sql, Action<SqlCommand> bind, string op, string table) where T : new()
    {
        try
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
            {
                bind?.Invoke(cmd);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return DbMapper.MapToList<T>(reader);
                }
            }
        }
        catch (Exception ex)
        {
            LogFailure(op, table, ex);
            throw;
        }
    }

    private int ExecuteNonQuery(string sql, Action<SqlCommand> bindParams, string op, string table)
    {
        try
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
            {
                bindParams?.Invoke(cmd);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            LogFailure(op, table, ex);
            throw;
        }
    }

    private object ExecuteScalar(string sql, Action<SqlCommand> bindParams, string op, string table)
    {
        try
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
            {
                bindParams?.Invoke(cmd);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
        catch (Exception ex)
        {
            LogFailure(op, table, ex);
            throw;
        }
    }

    private void LogFailure(string op, string table, Exception ex)
    {
        string msg = "[" + op + "] Tabla=" + (table ?? "(desconocida)") + ". Error=" + (ex?.Message ?? "");
        DAL.Audit.BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msg);
    }

    public void RecalculateTableDvsFromSelectAll(string tableName, string idColumn)
        => RecalculateTableDvsFromSelectAllCore(tableName, idColumn, suppressDvErrorLog: false);

    public void RecalculateTableDvsFromSelectAllSilent(string tableName, string idColumn)
        => RecalculateTableDvsFromSelectAllCore(tableName, idColumn, suppressDvErrorLog: true);

    private void RecalculateTableDvsFromSelectAllCore(string tableName, string idColumn, bool suppressDvErrorLog)
    {
        var rowValues = new List<(object IdValue, string DvhCalc)>();
        var dvhs = new List<string>();

        try
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM " + tableName + ";", conn) { CommandType = CommandType.Text })
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.HasRows)
                    {
                        conn.Close();

                        string currentDvv = GetCurrentDvv(tableName);
                        if (!string.Equals(currentDvv ?? string.Empty, string.Empty, StringComparison.Ordinal)
                            && !suppressDvErrorLog)
                        {
                            string msg = "Error en dígito verificador vertical en: " + tableName + ". Reparación realizada.";
                            DAL.Audit.BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msg);
                            DAL.Audit.BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.ReparacionIntegridadDatos, msg);
                        }

                        UpsertDvvWithDvh(tableName, string.Empty);
                        return;
                    }

                    var cols = new List<Tuple<string, int>>();
                    int idOrdinal = -1;
                    int dvhOrdinal = -1;

                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        var colName = rdr.GetName(i);

                        if (string.Equals(colName, "DVH", StringComparison.OrdinalIgnoreCase))
                        {
                            dvhOrdinal = i; // no 
                            continue;
                        }

                        if (string.Equals(colName, idColumn, StringComparison.OrdinalIgnoreCase))
                            idOrdinal = i;

                        cols.Add(Tuple.Create(colName, i));
                    }

                    if (idOrdinal < 0)
                        throw new InvalidOperationException("No se encontró la columna Id '" + idColumn + "' en " + tableName + ".");

                    cols.Sort((a, b) => string.Compare(a.Item1, b.Item1, StringComparison.Ordinal));

                    int mismatchesDvh = 0;

                    while (rdr.Read())
                    {
                        var sb = new StringBuilder();

                        for (int c = 0; c < cols.Count; c++)
                        {
                            int ord = cols[c].Item2;
                            object v = rdr.IsDBNull(ord) ? null : rdr.GetValue(ord);

                            if (v == null)
                            {
                            }
                            else if (v is byte[] bytes)
                            {
                                for (int k = 0; k < bytes.Length; k++)
                                    sb.Append(bytes[k].ToString("x2"));
                            }
                            else
                            {
                                sb.Append(Convert.ToString(v, CultureInfo.InvariantCulture));
                            }
                        }

                        string dvhCalc = SimpleDv(sb.ToString());
                        dvhs.Add(dvhCalc);

                        if (dvhOrdinal >= 0)
                        {
                            var dvhDb = rdr.IsDBNull(dvhOrdinal) ? string.Empty : Convert.ToString(rdr.GetValue(dvhOrdinal));
                            if (!string.Equals(dvhDb ?? string.Empty, dvhCalc ?? string.Empty, StringComparison.Ordinal))
                                mismatchesDvh++;
                        }

                        object idValue = rdr.GetValue(idOrdinal);
                        rowValues.Add((idValue, dvhCalc));
                    }

                    conn.Close();


                    string dvvCalc = ComputeDvv(dvhs);
                    string dvvDb = GetCurrentDvv(tableName);
                    bool dvvMatch = string.Equals(dvvDb ?? string.Empty, dvvCalc ?? string.Empty, StringComparison.Ordinal);

                    if (!suppressDvErrorLog)
                    {
                        if (mismatchesDvh > 0)
                        {
                            string msgH = "Error en dígito verificador horizontal en: " + tableName + ". Reparación realizada.";
                            DAL.Audit.BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msgH);
                            DAL.Audit.BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.ReparacionIntegridadDatos, msgH);
                        }
                        if (!dvvMatch)
                        {
                            string msgV = "Error en dígito verificador vertical en: " + tableName + ". Reparación realizada.";
                            DAL.Audit.BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msgV);
                            DAL.Audit.BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.ReparacionIntegridadDatos, msgV);
                        }
                    }
                }
            }

            for (int i = 0; i < rowValues.Count; i++)
                UpdateRowDvh(tableName, idColumn, rowValues[i].IdValue, rowValues[i].DvhCalc);

            string newDvv = ComputeDvv(dvhs);
            UpsertDvvWithDvh(tableName, newDvv);
        }
        catch (Exception ex)
        {
            LogFailure("DV-REFRESH", tableName, ex);
            throw;
        }
    }

    private void UpdateRowDvh(string table, string idColumn, object idValue, string dvh)
    {
        const string sqlTpl = "UPDATE {0} SET DVH = @dvh WHERE {1} = @id;";
        var sql = string.Format(sqlTpl, table, idColumn);

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@dvh", SqlDbType.VarChar, 256).Value = (object)dvh ?? string.Empty;
            cmd.Parameters.Add("@id", SqlDbType.Variant).Value = idValue ?? DBNull.Value;
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    private void UpsertDvvWithDvh(string tableName, string dvv)
    {
        var key = DvvKey(tableName);

        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();

            using (var upd = new SqlCommand(@"
UPDATE " + dvvTable + @"
   SET " + dvvColValue + @" = @dvv
 WHERE " + dvvColTableName + @" = @tabla;", conn))
            {
                upd.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
                upd.Parameters.Add("@dvv", SqlDbType.VarChar, 256).Value = dvv ?? string.Empty;

                int rows = upd.ExecuteNonQuery();
                if (rows == 0)
                {
                    using (var ins = new SqlCommand(@"
INSERT INTO " + dvvTable + @" (" + dvvColTableName + "," + dvvColValue + @")
VALUES (@tabla, @dvv);", conn))
                    {
                        ins.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
                        ins.Parameters.Add("@dvv", SqlDbType.VarChar, 256).Value = dvv ?? string.Empty;
                        ins.ExecuteNonQuery();
                    }
                }
            }

            // Actualizar DVH de la fila DVV (tabla + valorDVV)
            var dvh = SimpleDv((key ?? "") + (dvv ?? ""));
            using (var updDvh = new SqlCommand(@"
UPDATE " + dvvTable + @"
   SET " + dvvColDvh + @" = @dvh
 WHERE " + dvvColTableName + @" = @tabla;", conn))
            {
                updDvh.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
                updDvh.Parameters.Add("@dvh", SqlDbType.VarChar, 256).Value = dvh ?? string.Empty;
                updDvh.ExecuteNonQuery();
            }
        }
    }

    private string GetCurrentDvv(string tableName)
    {
        var key = DvvKey(tableName);
        var sql = "SELECT " + dvvColValue + " FROM " + dvvTable + " WHERE " + dvvColTableName + " = @tabla;";

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
        {
            cmd.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
            conn.Open();
            object o = cmd.ExecuteScalar();
            return (o == null || o == DBNull.Value) ? null : Convert.ToString(o);
        }
    }

    private static string DvvKey(string tableName)
    {
        if (string.IsNullOrEmpty(tableName)) return string.Empty;
        int dot = tableName.LastIndexOf('.');
        var name = dot >= 0 ? tableName.Substring(dot + 1) : tableName;
        return name.ToLowerInvariant();
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
}
