using System;
using System.Collections.Generic;
using System.IO;
using DAL.Mantenimiento;

namespace BLL.Seguridad.Mantenimiento
{
    public sealed class BackupBLL
    {
        private static BackupBLL _instance;
        private BackupBLL() { }

        public static BackupBLL GetInstance()
        {
            if (_instance == null) _instance = new BackupBLL();
            return _instance;
        }

        private static string DesktopDriveRoot()
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var root = Path.GetPathRoot(desktop);
            if (string.IsNullOrWhiteSpace(root) || root.Length < 2 || root[1] != ':')
                throw new InvalidOperationException("No se pudo resolver la unidad del Escritorio.");
            return root.Substring(0, 2);
        }

        public List<string> BackupFull(int parts = 1)
        {
            ValidarPartes(parts);
            var drive = DesktopDriveRoot();
            return BackupDAL.GetInstance().BackupFull(drive, parts);
        }

        public List<string> BackupFull(string destinationPath, int parts = 1)
        {
            ValidarPartes(parts);
            var destino = string.IsNullOrWhiteSpace(destinationPath)
                ? DesktopDriveRoot()
                : destinationPath.Trim();

            return BackupDAL.GetInstance().BackupFull(destino, parts);
        }

        public List<string> BackupToDrive(string driveLetter, int parts = 1)
        {
            if (string.IsNullOrWhiteSpace(driveLetter) || driveLetter.Length < 2 || driveLetter[1] != ':')
                throw new ArgumentException("Letra de unidad inválida. Ejemplo válido: C:", nameof(driveLetter));

            return BackupFull(driveLetter.Substring(0, 2), parts);
        }

        public List<string> BackupToFolder(string folderPath, int parts = 1)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("Debe indicar una carpeta de destino.", nameof(folderPath));

            folderPath = folderPath.Trim();
            return BackupFull(folderPath, parts);
        }

        public void RestoreFull(List<string> sourceFiles, bool withReplace = false, bool verifyBefore = true)
        {
            if (sourceFiles == null || sourceFiles.Count == 0)
                throw new ArgumentException("Debe indicar al menos un archivo de origen.", nameof(sourceFiles));

            BackupDAL.GetInstance().RestoreFull(sourceFiles, withReplace, verifyBefore);
        }

        private static void ValidarPartes(int parts)
        {
            if (parts < 1 || parts > 5)
                throw new ArgumentOutOfRangeException(nameof(parts), "El número de partes debe estar entre 1 y 5.");
        }
    }
}
