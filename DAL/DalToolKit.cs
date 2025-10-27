using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Linq;
using BitacoraDAL = DAL.Audit.BitacoraDAL;

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
        string accion,
        string mensaje,
        bool shouldCalculate = false)
    {
        int rows = ExecuteNonQuery(sql, bindParams, "NONQUERY", tableName);
        if (shouldCalculate)
            RecalculateTableDvsFromSelectAll(tableName, pkName);

        BitacoraDAL.GetInstance().Log(accion, mensaje);
        return rows;
    }

    public int ExecuteNonQueryAndLog(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName, string pkName, object pkValue,
        string accion,
        string mensaje,
        bool shouldCalculate = false)
    {
        int rows = ExecuteNonQuery(sql, bindParams, "NONQUERY", tableName);
        if (shouldCalculate)
        {
            if (pkValue != null)
                RefreshRowDvAndTableDvv(tableName, pkName, pkValue, silent: false);
            else
                RecalculateTableDvsFromSelectAll(tableName, pkName);
        }
        BitacoraDAL.GetInstance().Log(accion, mensaje);
        return rows;
    }

    public object ExecuteScalarAndLog(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName, string pkName,
        string accion,
        string mensaje,
        bool shouldCalculate = false)
    {
        object obj = ExecuteScalar(sql, bindParams, "SCALAR", tableName);
        if (shouldCalculate)
            RecalculateTableDvsFromSelectAll(tableName, pkName);

        BitacoraDAL.GetInstance().Log(accion, mensaje);
        return obj;
    }

    public object ExecuteScalarAndLog(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName, string pkName, object pkValue,
        string accion,
        string mensaje,
        bool shouldCalculate = false)
    {
        object obj = ExecuteScalar(sql, bindParams, "SCALAR", tableName);
        if (shouldCalculate)
        {
            if (pkValue != null)
                RefreshRowDvAndTableDvv(tableName, pkName, pkValue, silent: false);
            else
                RecalculateTableDvsFromSelectAll(tableName, pkName);
        }
        BitacoraDAL.GetInstance().Log(accion, mensaje);
        return obj;
    }

    public List<T> QueryListAndLog<T>(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn,
        string accion,
        string mensaje,
        bool shouldCalculate = false) where T : new()
    {
        var rows = ExecuteList<T>(sql, bindParams, "SELECT", tableName);
        if (shouldCalculate)
            RecalculateTableDvsFromSelectAll(tableName, idColumn);

        BitacoraDAL.GetInstance().Log(accion, mensaje);
        return rows;
    }

    public T QuerySingleOrDefaultAndLog<T>(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn,
        string accion,
        string mensaje,
        bool shouldCalculate = false) where T : new()
    {
        var list = QueryListAndLog<T>(sql, bindParams, tableName, idColumn, accion, mensaje, shouldCalculate);
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
        BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msg);
    }

    public void RecalculateTableDvsFromSelectAll(string tableName, string _ignoredIdColumn)
        => RecalculateTableDvsFromSelectAllComposite(tableName, suppressDvErrorLog: false);

    public void RecalculateTableDvsFromSelectAllSilent(string tableName, string _ignoredIdColumn)
        => RecalculateTableDvsFromSelectAllComposite(tableName, suppressDvErrorLog: true);

    public void RecalculateRowDvh(string tableName, string pkName, object pkValue)
    {
        if (pkValue == null) throw new ArgumentNullException(nameof(pkValue));

        string sql = "SELECT * FROM " + tableName + " WHERE " + pkName + " = @id;";
        string dvhCalc;

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
        {
            cmd.Parameters.Add("@id", SqlDbType.Variant).Value = pkValue;
            conn.Open();
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (!rdr.Read())
                    throw new InvalidOperationException("No existe la fila con PK especificada para " + tableName + ".");

                var cols = new List<Tuple<string, int>>();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    var name = rdr.GetName(i);
                    if (string.Equals(name, "DVH", StringComparison.OrdinalIgnoreCase))
                        continue;
                    cols.Add(Tuple.Create(name, i));
                }
                cols.Sort((a, b) => string.Compare(a.Item1, b.Item1, StringComparison.Ordinal));

                var sb = new StringBuilder();
                for (int c = 0; c < cols.Count; c++)
                {
                    int ord = cols[c].Item2;
                    object v = rdr.IsDBNull(ord) ? null : rdr.GetValue(ord);

                    if (v == null) { }
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

                dvhCalc = SimpleDv(sb.ToString());
            }
        }

        UpdateRowDvh_SinglePk(tableName, pkName, pkValue, dvhCalc);
    }

    public void RecalculateDvvFromExistingDvhs(string tableName, bool silent = false)
    {
        var dvhs = new List<string>();

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("SELECT DVH FROM " + tableName + ";", conn) { CommandType = CommandType.Text })
        {
            conn.Open();
            using (var rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows)
                {
                    conn.Close();
                    string current = GetCurrentDvv(tableName);
                    if (!string.Equals(current ?? string.Empty, string.Empty, StringComparison.Ordinal) && !silent)
                    {
                        string msg = "Error en dígito verificador vertical en: " + tableName + ". Reparación realizada.";
                        BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msg);
                        BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.ReparacionIntegridadDatos, msg);
                    }
                    UpsertDvvWithDvh(tableName, string.Empty);
                    return;
                }

                while (rdr.Read())
                {
                    var dvh = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    dvhs.Add(dvh ?? string.Empty);
                }
            }
        }

        string dvvCalc = ComputeDvv(dvhs);
        string dvvDb = GetCurrentDvv(tableName);
        if (!string.Equals(dvvDb ?? string.Empty, dvvCalc ?? string.Empty, StringComparison.Ordinal) && !silent)
        {
            string msgV = "Error en dígito verificador vertical en: " + tableName + ". Reparación realizada.";
            BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msgV);
            BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.ReparacionIntegridadDatos, msgV);
        }

        UpsertDvvWithDvh(tableName, dvvCalc);
    }

    public void RefreshRowDvAndTableDvv(string tableName, string pkName, object pkValue, bool silent = false)
    {
        RecalculateRowDvh(tableName, pkName, pkValue);
        RecalculateDvvFromExistingDvhs(tableName, silent);
    }

    private void UpdateRowDvh_SinglePk(string table, string idColumn, object idValue, string dvh)
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

    private static string NormalizeKey(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) return string.Empty;
        var t = tableName.Trim().Replace("[", "").Replace("]", "");
        var parts = t.Split('.');
        var name = parts.Length >= 2 ? parts[parts.Length - 1] : parts[0];
        return name.ToLowerInvariant();
    }

    private static string DvvKey(string tableName) => NormalizeKey(tableName);

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

    private sealed class TablePkInfoComposite
    {
        public string TableFullName { get; set; }               // ej: [dbo].[UsuarioFamilia]
        public List<string> PkColumnNames { get; set; } = new List<string>();  // ej: idUsuario, idFamilia
    }

    private List<TablePkInfoComposite> EnumerateTablesWithAnyPkAndDvh(params string[] extraExcludes)
    {
        var result = new List<TablePkInfoComposite>();
        var excludes = new HashSet<string>(
            new[] { "[dbo].[DigitoVerificadorVertical]" }
            .Concat(extraExcludes ?? Array.Empty<string>()),
            StringComparer.OrdinalIgnoreCase);

        const string sql = @"
WITH PkCols AS (
    SELECT
        tc.TABLE_SCHEMA,
        tc.TABLE_NAME,
        kcu.COLUMN_NAME,
        kcu.ORDINAL_POSITION
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
      ON kcu.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA
     AND kcu.CONSTRAINT_NAME   = tc.CONSTRAINT_NAME
    WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
),
HasDvh AS (
    SELECT DISTINCT TABLE_SCHEMA, TABLE_NAME
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE COLUMN_NAME = 'DVH'
),
BaseTables AS (
    SELECT TABLE_SCHEMA, TABLE_NAME
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_TYPE = 'BASE TABLE'
)
SELECT
    b.TABLE_SCHEMA,
    b.TABLE_NAME,
    p.COLUMN_NAME,
    p.ORDINAL_POSITION
FROM BaseTables b
JOIN PkCols p
  ON p.TABLE_SCHEMA = b.TABLE_SCHEMA AND p.TABLE_NAME = b.TABLE_NAME
JOIN HasDvh h
  ON h.TABLE_SCHEMA = b.TABLE_SCHEMA AND h.TABLE_NAME = b.TABLE_NAME
ORDER BY b.TABLE_SCHEMA, b.TABLE_NAME, p.ORDINAL_POSITION;";

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
        {
            conn.Open();
            using (var rdr = cmd.ExecuteReader())
            {
                var map = new Dictionary<string, List<(int Ord, string Col)>>(StringComparer.OrdinalIgnoreCase);

                while (rdr.Read())
                {
                    var schema = rdr.GetString(0);
                    var name = rdr.GetString(1);
                    var col = rdr.GetString(2);
                    var ord = rdr.IsDBNull(3) ? 0 : Convert.ToInt32(rdr.GetValue(3));

                    var full = $"[{schema}].[{name}]";
                    if (excludes.Contains(full)) continue;

                    if (!map.TryGetValue(full, out var list))
                    {
                        list = new List<(int Ord, string Col)>();
                        map[full] = list;
                    }
                    list.Add((ord, col));
                }

                foreach (var kv in map)
                {
                    var pkCols = kv.Value
                                   .OrderBy(x => x.Ord)
                                   .Select(x => x.Col)
                                   .Where(c => !string.IsNullOrWhiteSpace(c))
                                   .ToList();

                    if (pkCols.Count == 0) continue;

                    result.Add(new TablePkInfoComposite
                    {
                        TableFullName = kv.Key,
                        PkColumnNames = pkCols
                    });
                }
            }
        }
        return result;
    }

    private (string schema, string name) ParseSchemaAndName(string tableFullName)
    {
        var t = (tableFullName ?? "").Trim();
        t = t.Replace("[", "").Replace("]", "");
        var parts = t.Split('.');
        if (parts.Length == 2) return (parts[0], parts[1]);
        return ("dbo", t);
    }

    private List<string> GetPkColumnsFromDb(string tableFullName)
    {
        var (schema, name) = ParseSchemaAndName(tableFullName);
        const string sql = @"
SELECT kcu.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
  ON kcu.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA
 AND kcu.CONSTRAINT_NAME   = tc.CONSTRAINT_NAME
WHERE tc.TABLE_SCHEMA = @s AND tc.TABLE_NAME = @n AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
ORDER BY kcu.ORDINAL_POSITION;";

        var list = new List<string>();
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
        {
            cmd.Parameters.Add("@s", SqlDbType.VarChar, 128).Value = schema;
            cmd.Parameters.Add("@n", SqlDbType.VarChar, 128).Value = name;
            conn.Open();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    list.Add(rdr.GetString(0));
            }
        }
        return list;
    }

    private void RecalculateTableDvsFromSelectAllComposite(string tableFullName, bool suppressDvErrorLog)
    {
        var pkColumns = GetPkColumnsFromDb(tableFullName);
        if (pkColumns.Count == 0) return;

        var dvhs = new List<string>();
        var rowKeys = new List<Dictionary<string, object>>();

        try
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM " + tableFullName + ";", conn) { CommandType = CommandType.Text })
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.HasRows)
                    {
                        conn.Close();
                        string currentDvv = GetCurrentDvv(tableFullName);
                        if (!string.Equals(currentDvv ?? string.Empty, string.Empty, StringComparison.Ordinal)
                            && !suppressDvErrorLog)
                        {
                            string msg = "Error en dígito verificador vertical en: " + tableFullName + ". Reparación realizada.";
                            BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msg);
                            BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.ReparacionIntegridadDatos, msg);
                        }

                        UpsertDvvWithDvh(tableFullName, string.Empty);
                        return;
                    }

                    var cols = new List<Tuple<string, int>>();
                    int dvhOrdinal = -1;

                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        var colName = rdr.GetName(i);
                        if (string.Equals(colName, "DVH", StringComparison.OrdinalIgnoreCase))
                        {
                            dvhOrdinal = i;
                            continue;
                        }
                        cols.Add(Tuple.Create(colName, i));
                    }

                    cols.Sort((a, b) => string.Compare(a.Item1, b.Item1, StringComparison.Ordinal));

                    int mismatchesDvh = 0;

                    while (rdr.Read())
                    {
                        var sb = new StringBuilder();

                        for (int c = 0; c < cols.Count; c++)
                        {
                            int ord = cols[c].Item2;
                            object v = rdr.IsDBNull(ord) ? null : rdr.GetValue(ord);

                            if (v == null) { }
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

                        var key = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        foreach (var pk in pkColumns)
                        {
                            int ord = rdr.GetOrdinal(pk);
                            key[pk] = rdr.IsDBNull(ord) ? null : rdr.GetValue(ord);
                        }
                        rowKeys.Add(key);
                    }

                    conn.Close();

                    if (!suppressDvErrorLog && mismatchesDvh > 0)
                    {
                        string msgH = "Error en dígito verificador horizontal en: " + tableFullName + ". Reparación realizada.";
                        BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad, msgH);
                        BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.ReparacionIntegridadDatos, msgH);
                    }
                }
            }

            for (int i = 0; i < rowKeys.Count; i++)
                UpdateRowDvhComposite(tableFullName, pkColumns, rowKeys[i], dvhs[i]);

            string newDvv = ComputeDvv(dvhs);
            UpsertDvvWithDvh(tableFullName, newDvv);
        }
        catch (Exception ex)
        {
            LogFailure("DV-REFRESH", tableFullName, ex);
            throw;
        }
    }

    private void UpdateRowDvhComposite(string tableFullName, List<string> pkColumns, Dictionary<string, object> keyValues, string dvh)
    {
        var where = string.Join(" AND ", pkColumns.Select((pk, i) => $"[{pk}] = @pk{i}"));
        var sql = $"UPDATE {tableFullName} SET DVH = @dvh WHERE {where};";

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text })
        {
            cmd.Parameters.Add("@dvh", SqlDbType.VarChar, 256).Value = (object)dvh ?? string.Empty;

            int i = 0;
            foreach (var pk in pkColumns)
            {
                object val = keyValues.TryGetValue(pk, out var v) ? v : DBNull.Value;
                cmd.Parameters.Add($"@pk{i}", SqlDbType.Variant).Value = val ?? DBNull.Value;
                i++;
            }

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void VerifyAndRepairAllTablesAuto(params string[] extraExcludes)
    {
        var tables = EnumerateTablesWithAnyPkAndDvh(extraExcludes);
        for (int i = 0; i < tables.Count; i++)
        {
            var t = tables[i];
            try
            {
                RecalculateTableDvsFromSelectAllComposite(t.TableFullName, suppressDvErrorLog: false);
            }
            catch
            {
                // nada
            }
        }
    }
}
