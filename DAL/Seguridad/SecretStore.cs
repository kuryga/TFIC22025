using System;
using System.IO;
using System.Text;

namespace DAL.Seguridad
{
    public static class ConnectionSecretStore
    {
        private static readonly string SecretPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                         "UrbanSoft", "conn.secret");

        public static string SecretFilePath => SecretPath;

        public static void SaveConnectionString(string plainConnectionString)
        {
            if (string.IsNullOrWhiteSpace(plainConnectionString))
                throw new ArgumentException("Connection string vacío.", nameof(plainConnectionString));

            var dir = Path.GetDirectoryName(SecretPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var encrypted = SecurityUtilities.EncriptarReversible(plainConnectionString);

            File.WriteAllText(SecretPath, encrypted, Encoding.UTF8);
        }

        public static bool TryLoad(out string connectionString)
        {
            connectionString = null;

            if (!File.Exists(SecretPath))
                return false;

            try
            {
                var encrypted = File.ReadAllText(SecretPath, Encoding.UTF8)?.Trim();
                if (string.IsNullOrWhiteSpace(encrypted))
                    return false;

                connectionString = SecurityUtilities.DesencriptarReversible(encrypted);
                return !string.IsNullOrWhiteSpace(connectionString);
            }
            catch
            {
                return false;
            }
        }

        public static string LoadOrThrow()
        {
            if (!TryLoad(out var cs))
                throw new InvalidOperationException(
                    $"No se pudo leer el archivo de conexión cifrado. Ruta: {SecretPath}. " +
                    "Genere o actualice el archivo desde la utilidad de configuración.");

            return cs;
        }
    }
}
