using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using BitacoraDAL = DAL.Audit.BitacoraDAL;

namespace DAL.Mantenimiento
{
    public sealed class BackupDAL
    {
        private static BackupDAL instance;
        private BackupDAL() { }

        public static BackupDAL GetInstance()
        {
            if (instance == null) instance = new BackupDAL();
            return instance;
        }

        private static string ConnStr => DalToolkit.connectionString;

        // Construye una connection string cambiando el catálogo.
        private static string GetConnStrFor(string catalog)
        {
            var csb = new SqlConnectionStringBuilder(ConnStr);
            csb.InitialCatalog = catalog;
            return csb.ToString();
        }

        private static string CurrentDbName
        {
            get
            {
                using (var cn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("SELECT DB_NAME();", cn))
                {
                    cn.Open();
                    var name = Convert.ToString(cmd.ExecuteScalar(), CultureInfo.InvariantCulture);
                    return string.IsNullOrWhiteSpace(name) ? "UrbanSoft" : name;
                }
            }
        }

        public List<string> BackupFull(string destinationPath, int parts = 1)
        {
            if (string.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("Debe indicar una unidad o carpeta de destino.", nameof(destinationPath));
            if (parts < 1 || parts > 5)
                throw new ArgumentOutOfRangeException(nameof(parts), "El número de partes debe estar entre 1 y 5.");

            var dbName = CurrentDbName;
            var now = DateTime.Now;
            var stamp = now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            var baseName = $"{dbName}_FULL_{stamp}";

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fallbackFolder = Path.Combine(desktopPath, "Backups");
            string backupFolder;

            try
            {
                if (Directory.Exists(destinationPath))
                {
                    backupFolder = destinationPath;
                }
                else if (destinationPath.Length >= 2 && destinationPath[1] == ':')
                {
                    backupFolder = Path.Combine(destinationPath.TrimEnd('\\') + "\\", "Backups");
                }
                else
                {
                    backupFolder = fallbackFolder;
                }

                if (!Directory.Exists(backupFolder))
                    Directory.CreateDirectory(backupFolder);
            }
            catch
            {
                backupFolder = fallbackFolder;
                if (!Directory.Exists(backupFolder))
                    Directory.CreateDirectory(backupFolder);
            }

            var files = new List<string>(parts);
            for (int i = 1; i <= parts; i++)
            {
                var suffix = parts == 1 ? "" : $"_p{i}";
                var path = Path.Combine(backupFolder, $"{baseName}{suffix}.bak");
                files.Add(path);
            }

            var toClauses = string.Join(", ", files.ConvertAll(f => $"DISK = N'{f.Replace("'", "''")}'"));
            var sql = $"BACKUP DATABASE [{dbName}] TO {toClauses} WITH COMPRESSION, STATS = 5, SKIP;";
            ExecNonQuery(sql, overrideCatalog: dbName); // backup puede ejecutarse desde la misma BD

            var verifyAll = $"RESTORE VERIFYONLY FROM {toClauses};";
            ExecNonQuery(verifyAll, overrideCatalog: "master"); // verify desde master para no bloquear

            BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.RespaldoBaseDatos,
                $"Backup total de {dbName}. Partes {parts}. Carpeta {backupFolder}.");

            return files;
        }

        public void RestoreFull(List<string> sourceFiles, bool withReplace = false, bool verifyBefore = true)
        {
            if (sourceFiles == null || sourceFiles.Count == 0)
                throw new ArgumentException("Debe indicar al menos un archivo de origen.", nameof(sourceFiles));

            var dbName = CurrentDbName;
            var fromClauses = string.Join(", ", sourceFiles.ConvertAll(f => $"DISK = N'{f.Replace("'", "''")}'"));

            // Siempre trabajar contra master durante el restore
            var catalog = "master";

            if (verifyBefore)
            {
                var verifyAll = $"RESTORE VERIFYONLY FROM {fromClauses};";
                ExecNonQuery(verifyAll, overrideCatalog: catalog);
            }

            // Matar sesiones activas y forzar SINGLE_USER con reintento
            var dbNameEscaped = dbName.Replace("'", "''");
            string killSql = $@"
DECLARE @db sysname = N'{dbNameEscaped}';
DECLARE @kill nvarchar(max) = N'';

-- Construir lista de KILL para todas las sesiones excepto la actual
SELECT @kill = COALESCE(@kill, N'') + N'KILL ' + CONVERT(varchar(10), s.session_id) + N';'
FROM sys.dm_exec_sessions AS s
WHERE s.database_id = DB_ID(@db) AND s.session_id <> @@SPID;

-- Ejecutar los KILL (si hay)
IF (LEN(@kill) > 0) EXEC(@kill);

-- Intento 1: SINGLE_USER
BEGIN TRY
    EXEC(N'ALTER DATABASE [' + @db + N'] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;');
END TRY
BEGIN CATCH
    -- Pequeño delay y reintento: matar sesiones de nuevo
    WAITFOR DELAY '00:00:01';
    SET @kill = N'';
    SELECT @kill = COALESCE(@kill, N'') + N'KILL ' + CONVERT(varchar(10), s.session_id) + N';'
    FROM sys.dm_exec_sessions AS s
    WHERE s.database_id = DB_ID(@db) AND s.session_id <> @@SPID;
    IF (LEN(@kill) > 0) EXEC(@kill);

    EXEC(N'ALTER DATABASE [' + @db + N'] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;');
END CATCH;
";
            ExecNonQuery(killSql, overrideCatalog: catalog);

            try
            {
                var replace = withReplace ? ", REPLACE" : "";
                var sql = $"RESTORE DATABASE [{dbName}] FROM {fromClauses} WITH RECOVERY{replace}, STATS = 5;";
                ExecNonQuery(sql, overrideCatalog: catalog);
            }
            finally
            {
                // Volver a multi user aunque falle el restore
                var toMultiUser = $"ALTER DATABASE [{dbName}] SET MULTI_USER;";
                try { ExecNonQuery(toMultiUser, overrideCatalog: catalog); } catch { /* best effort */ }
            }

            BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.RestauracionBaseDatos,
                $"Restore total de {dbName} desde {sourceFiles.Count} archivo(s). REPLACE {(withReplace ? "SI" : "NO")}.");
        }

        private static void ExecNonQuery(string sql, string overrideCatalog = null)
        {
            try
            {
                var cs = overrideCatalog == null ? ConnStr : GetConnStrFor(overrideCatalog);
                using (var cn = new SqlConnection(cs))
                using (var cmd = new SqlCommand(sql, cn) { CommandType = CommandType.Text, CommandTimeout = 0 })
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.FalloVerificacionIntegridad,
                    "[BACKUP/RESTORE] Error " + (ex?.Message ?? ""));
                throw;
            }
        }
    }
}
