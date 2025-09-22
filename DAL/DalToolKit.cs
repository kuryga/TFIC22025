using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;

namespace DAL
{

    public sealed class DalToolkit
    {
        // -------------- CONFIG GLOBAL --------------
        // Connection string fijo para todo el proyecto
        private const string connectionString =
            @"Server=.;Database=UrbanSoft;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        // Tabla DVV (según tu captura)
        private const string dvvTable = "[dbo].[DigitoVerificadorVertical]";
        private const string dvvColTableName = "tabla";
        private const string dvvColValue = "valorDVV";
        private const string dvvColDvh = "DVH";

        // -------------- API PÚBLICA --------------

        public List<T> QueryListAndUpdateDv<T>(
            string sql,
            Action<SqlCommand> bindParams,
            string tableName,
            string idColumn,
            Func<T, object> getId) where T : new()
        {
            var rows = ExecuteList<T>(sql, bindParams);

            var dvhs = new List<string>(rows.Count);
            for (int i = 0; i < rows.Count; i++)
            {
                var dvh = ComputeRowDvh(rows[i]);
                dvhs.Add(dvh);
                UpdateRowDvh(tableName, idColumn, getId(rows[i]), dvh);
            }

            // 2) DVV de la tabla + DVH de la fila en DVV
            var dvv = ComputeDvv(dvhs);
            UpsertDvvWithDvh(tableName, dvv);

            return rows;
        }

        public T QuerySingleOrDefaultAndUpdateDv<T>(
            string sql,
            Action<SqlCommand> bindParams,
            string tableName,
            string idColumn,
            Func<T, object> getId) where T : new()
        {
            var list = QueryListAndUpdateDv<T>(sql, bindParams, tableName, idColumn, getId);
            return list.Count > 0 ? list[0] : default(T); // null para clases
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
            var key = DvvKey(tableName);

            var conn = new SqlConnection(connectionString);
            conn.Open();

            var upd = new SqlCommand(@"UPDATE " + dvvTable + @"
                                       SET " + dvvColValue + @" = @dvv
                                     WHERE " + dvvColTableName + @" = @tabla;", conn);

            upd.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
            upd.Parameters.Add("@dvv", SqlDbType.VarChar, 256).Value = dvv ?? string.Empty;

            int rows = upd.ExecuteNonQuery();
            if (rows == 0)
            {
                var ins = new SqlCommand(@"INSERT INTO " + dvvTable + @" (" + dvvColTableName + "," + dvvColValue + @")
                                            VALUES (@tabla, @dvv);", conn);
                ins.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
                ins.Parameters.Add("@dvv", SqlDbType.VarChar, 256).Value = dvv ?? string.Empty;
                ins.ExecuteNonQuery();
            }

            // actualizar DVH de la fila DVV (tabla-normalizada + valorDVV)
            var dvh = ComputeDvhForDvvRow(key, dvv ?? string.Empty);
            var updDvh = new SqlCommand(@"UPDATE " + dvvTable + @"
                                         SET " + dvvColDvh + @" = @dvh
                                        WHERE " + dvvColTableName + @" = @tabla;", conn);

            updDvh.Parameters.Add("@tabla", SqlDbType.VarChar, 128).Value = key;
            updDvh.Parameters.Add("@dvh", SqlDbType.VarChar, 256).Value = dvh ?? string.Empty;
            updDvh.ExecuteNonQuery();

            conn.Close();
        }

        private static string ComputeRowDvh<T>(T row)
        {
            if (row == null) return string.Empty;

            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Array.Sort(props, (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

            var sb = new StringBuilder();
            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];
                if (!p.CanRead) continue;
                if (string.Equals(p.Name, "DVH", StringComparison.OrdinalIgnoreCase)) continue;

                var val = p.GetValue(row, null);
                sb.Append(val != null ? val.ToString() : string.Empty);
            }
            return SimpleDv(sb.ToString());
        }

        private static string ComputeDvv(List<string> dvhs)
        {
            string acc = string.Empty;
            for (int i = 0; i < dvhs.Count; i++)
                acc = SimpleDv(acc + (dvhs[i] ?? string.Empty));
            return acc;
        }

        private static string ComputeDvhForDvvRow(string tableName, string dvvValue)
        {
            return SimpleDv((tableName ?? "") + (dvvValue ?? ""));
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

        public int ExecuteNonQuery(string sql, Action<SqlCommand> bindParams)
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

        public object ExecuteScalar(string sql, Action<SqlCommand> bindParams)
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

        private static string DvvKey(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return string.Empty;
            int dot = tableName.LastIndexOf('.');
            var name = dot >= 0 ? tableName.Substring(dot + 1) : tableName;
            return name;
        }
    }
}
