using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

public sealed class DalToolkit
{
    // Connection string global
    private const string connectionString =
        @"Server=.;Database=UrbanSoft;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

    // Tabla DVV (según tu esquema)
    private const string dvvTable = "[dbo].[DigitoVerificadorVertical]";
    private const string dvvColTableName = "tabla";
    private const string dvvColValue = "valorDVV";
    private const string dvvColDvh = "DVH";

    // ---------------- API pública ----------------

    // Devuelve lista (según tu SQL) y luego recalcula DV con SELECT * (todas las columnas)
    public List<T> QueryListAndUpdateDv<T>(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn) where T : new()
    {
        var rows = ExecuteList<T>(sql, bindParams);
        RecalculateTableDvsFromSelectAll(tableName, idColumn);
        return rows;
    }

    // Devuelve 1 (o null) y luego recalcula DV con SELECT *
    public T QuerySingleOrDefaultAndUpdateDv<T>(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn) where T : new()
    {
        var list = QueryListAndUpdateDv<T>(sql, bindParams, tableName, idColumn);
        return list.Count > 0 ? list[0] : default(T);
    }

    // Ejecuta escritura y refresca DV con SELECT *
    public int ExecuteNonQueryAndRefresh(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn)
    {
        int rows = ExecuteNonQuery(sql, bindParams);
        RecalculateTableDvsFromSelectAll(tableName, idColumn);
        return rows;
    }

    // Ejecuta escalar (INSERT SCOPE_IDENTITY, etc.) y refresca DV con SELECT *
    public object ExecuteScalarAndRefresh(
        string sql,
        Action<SqlCommand> bindParams,
        string tableName,
        string idColumn)
    {
        object obj = ExecuteScalar(sql, bindParams);
        RecalculateTableDvsFromSelectAll(tableName, idColumn);
        return obj;
    }

    // Recalcula DVH por fila (UPDATE ... SET DVH=...) y DVV (upsert en DVV + su DVH) leyendo SELECT *
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

            // columnas (excluye DVH), identificar idColumn
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

            // orden determinístico por nombre de columna
            cols.Sort((a, b) => string.Compare(a.Item1, b.Item1, StringComparison.Ordinal));

            // recorrer filas → DVH por fila
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

            // DVV de la tabla (y su DVH en la tabla DVV)
            string dvv = ComputeDvv(dvhs);
            UpsertDvvWithDvh(tableName, dvv);
        }
        catch
        {
            if (conn.State == ConnectionState.Open) conn.Close();
            throw;
        }
    }

    // ---------------- internas ----------------

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
            result = DbMapper.MapToList<T>(reader); // tu mapper
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

    private void UpdateRowDvh(string table, string idColumn, object idValue, string dvh)
    {
        var sql = "UPDATE " + table + " SET DVH = @dvh WHERE " + idColumn + " = @id;";
        var conn = new SqlConnection(connectionString);
        var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@dvh", SqlDbType.VarChar, 256).Value = (object)dvh ?? string.Empty;
        cmd.Parameters.Add("@id", SqlDbType.Variant).Value = idValue ?? DBNull.Value;

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    private void UpsertDvvWithDvh(string tableName, string dvv)
    {
        var key = DvvKey(tableName); // "usuario"

        var conn = new SqlConnection(connectionString);
        conn.Open();

        // upsert valorDVV
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

        // actualizar DVH de la fila DVV (tabla-normalizada + valorDVV)
        var dvh = SimpleDv((key ?? "") + (dvv ?? ""));
        var updDvh = new SqlCommand(@"
UPDATE " + dvvTable + @"
   SET " + dvvColDvh + @" = @dvh
 WHERE " + dvvColTableName + @" = @tabla;", conn);

        updDvh.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
        updDvh.Parameters.Add("@dvh", SqlDbType.VarChar, 256).Value = dvh ?? string.Empty;
        updDvh.ExecuteNonQuery();

        conn.Close();
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
}
