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

        public List<string> BackupFull(string destinationDrive, int parts = 1)
        {
            if (string.IsNullOrWhiteSpace(destinationDrive) || destinationDrive.Length < 2 || destinationDrive[1] != ':')
                throw new ArgumentException("Unidad de destino inválida.", nameof(destinationDrive));
            if (parts < 1 || parts > 5)
                throw new ArgumentOutOfRangeException(nameof(parts), "El número de partes debe estar entre 1 y 5.");

            var dbName = CurrentDbName;
            var now = DateTime.Now;
            var stamp = now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            var baseName = $"{dbName}_FULL_{stamp}";

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var backupFolder = Path.Combine(desktopPath, "Backups");
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);

            var files = new List<string>(parts);
            for (int i = 1; i <= parts; i++)
            {
                var suffix = parts == 1 ? "" : $"_p{i}";
                var path = Path.Combine(backupFolder, $"{baseName}{suffix}.bak");
                files.Add(path);
            }

            var toClauses = string.Join(", ", files.ConvertAll(f => $"DISK = N'{f.Replace("'", "''")}'"));
            var sql = $"BACKUP DATABASE [{dbName}] TO {toClauses} WITH COMPRESSION, STATS = 5, SKIP;";
            ExecNonQuery(sql);

            foreach (var f in files)
            {
                var verify = $"RESTORE VERIFYONLY FROM DISK = N'{f.Replace("'", "''")}';";
                ExecNonQuery(verify);
            }

            BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.RespaldoBaseDatos,
                $"Backup total de {dbName}. Partes {parts}. Carpeta {backupFolder}.");

            return files;
        }

        public void RestoreFull(List<string> sourceFiles, bool withReplace = false, bool verifyBefore = true)
        {
            if (sourceFiles == null || sourceFiles.Count == 0)
                throw new ArgumentException("Debe indicar al menos un archivo de origen.", nameof(sourceFiles));

            var dbName = CurrentDbName;

            if (verifyBefore)
            {
                foreach (var f in sourceFiles)
                {
                    var verify = $"RESTORE VERIFYONLY FROM DISK = N'{f.Replace("'", "''")}';";
                    ExecNonQuery(verify);
                }
            }

            var fromClauses = string.Join(", ", sourceFiles.ConvertAll(f => $"DISK = N'{f.Replace("'", "''")}'"));
            var replace = withReplace ? ", REPLACE" : "";
            var sql = $"RESTORE DATABASE [{dbName}] FROM {fromClauses} WITH RECOVERY{replace}, STATS = 5;";
            ExecNonQuery(sql);

            BitacoraDAL.GetInstance().Log(BE.Audit.AuditEvents.RestauracionBaseDatos,
                $"Restore total de {dbName} desde {sourceFiles.Count} archivo(s). REPLACE {(withReplace ? "SI" : "NO")}.");
        }

        private static void ExecNonQuery(string sql)
        {
            try
            {
                using (var cn = new SqlConnection(ConnStr))
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
