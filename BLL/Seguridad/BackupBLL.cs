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
            var drive = DesktopDriveRoot();
            return BackupDAL.GetInstance().BackupFull(drive, parts);
        }

        public void RestoreFull(List<string> sourceFiles, bool withReplace = false, bool verifyBefore = true)
        {
            if (sourceFiles == null || sourceFiles.Count == 0)
                throw new ArgumentException("Debe indicar al menos un archivo de origen.", nameof(sourceFiles));
            BackupDAL.GetInstance().RestoreFull(sourceFiles, withReplace, verifyBefore);
        }
    }
}
