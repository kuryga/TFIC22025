using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace UI.Helpers
{
    public static class SecretStore
    {
        private static readonly string SecretPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                         "UrbanSoft", "conn.secret");

        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("UrbanSoft-ConnString-v1");

        public static string GetConnectionStringOrThrow()
        {
            if (!File.Exists(SecretPath))
                throw new InvalidOperationException(
                    $"No se encontró el archivo de secreto: {SecretPath}. " +
                    "Genere el archivo de conexión desde la utilidad de configuración.");

            var cipher = File.ReadAllBytes(SecretPath);
            var plain = ProtectedData.Unprotect(cipher, Entropy, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(plain);
        }

        public static void SaveConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string vacío.");

            var dir = Path.GetDirectoryName(SecretPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var plain = Encoding.UTF8.GetBytes(connectionString);
            var cipher = ProtectedData.Protect(plain, Entropy, DataProtectionScope.LocalMachine);
            File.WriteAllBytes(SecretPath, cipher);
        }

        public static string SecretFilePath => SecretPath;
    }

}
