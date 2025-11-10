using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DAL.Seguridad
{
    public static class SecuritySecretStore
    {
        private static readonly string SecretPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                         "UrbanSoft", "sec.secret");

        public static string SecretFilePath => SecretPath;

        private sealed class SecData
        {
            public string DefaultPassphrase { get; set; }
            public string AesSaltText { get; set; }
            public int? Iterations { get; set; }  // opcional
        }

        public static void SaveSecrets(string defaultPassphrase, string aesSaltText, int iterations = 100000)
        {
            if (string.IsNullOrWhiteSpace(defaultPassphrase))
                throw new ArgumentException("Passphrase vacío.", nameof(defaultPassphrase));
            if (string.IsNullOrWhiteSpace(aesSaltText))
                throw new ArgumentException("Salt vacío.", nameof(aesSaltText));
            if (iterations <= 0) iterations = 100000;

            var dir = Path.GetDirectoryName(SecretPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var data = new SecData
            {
                DefaultPassphrase = defaultPassphrase,
                AesSaltText = aesSaltText,
                Iterations = iterations
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SecretPath, json, Encoding.UTF8);
        }

        public static bool TryLoad(out string defaultPassphrase, out string aesSaltText, out int iterations)
        {
            defaultPassphrase = null;
            aesSaltText = null;
            iterations = 100000;

            if (!File.Exists(SecretPath))
                return false;

            try
            {
                var content = File.ReadAllText(SecretPath, Encoding.UTF8)?.Trim();
                if (string.IsNullOrWhiteSpace(content))
                    return false;

                var data = JsonSerializer.Deserialize<SecData>(content);
                if (data == null ||
                    string.IsNullOrWhiteSpace(data.DefaultPassphrase) ||
                    string.IsNullOrWhiteSpace(data.AesSaltText))
                    return false;

                defaultPassphrase = data.DefaultPassphrase;
                aesSaltText = data.AesSaltText;
                iterations = (data.Iterations.HasValue && data.Iterations.Value > 0)
                                ? data.Iterations.Value
                                : 100000;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static (string DefaultPassphrase, string AesSaltText, int Iterations) LoadOrThrow()
        {
            string pass, salt;
            int iters;
            if (!TryLoad(out pass, out salt, out iters))
                throw new InvalidOperationException(
                    $"No se pudo leer el archivo de secretos. Ruta: {SecretPath}. " +
                    "Genere o actualice el archivo sec.secret con DefaultPassphrase y AesSaltText.");

            return (pass, salt, iters);
        }

        public static void InitSecurityKeyOrThrow()
        {
            var s = LoadOrThrow();
            SecurityUtilities.SetEncryptionKeyFromPassphrase(s.DefaultPassphrase, s.AesSaltText, s.Iterations);
        }
    }
}
