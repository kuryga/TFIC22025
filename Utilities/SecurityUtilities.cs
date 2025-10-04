using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Utilities
{
    public static class SecurityUtilities
    {
        private static byte[] _aesKey; // 32 bytes
        private const string DefaultPassphrase = "TFIUAI2025AAGK"; // TODO: mover al AppConfiguration
        private const string AesSaltText = "UrbanSoft-AES-Key-Salt"; // estático para derivar clave
        private const int AesDeriveIterations = 100_000;

        // PBKDF2 para contraseñas irreversible
        private const int PasswordSaltSize = 16;   // 128 bits
        private const int PasswordKeySize = 32;   // 256 bits
        private const int PasswordIterations = 100_000;


        static SecurityUtilities()
        {
            SetEncryptionKeyFromPassphrase_Default();
        }

        public static string EncriptarReversible(string textoPlano)
        {
            EnsureAesKey();

            if (textoPlano == null) textoPlano = string.Empty;

            using (var aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                byte[] plain = Encoding.UTF8.GetBytes(textoPlano);
                byte[] cipher;

                using (var enc = aes.CreateEncryptor())
                {
                    cipher = enc.TransformFinalBlock(plain, 0, plain.Length);
                }

                // Empaquetar IV || CIPHER
                var output = new byte[aes.IV.Length + cipher.Length];
                Buffer.BlockCopy(aes.IV, 0, output, 0, aes.IV.Length);
                Buffer.BlockCopy(cipher, 0, output, aes.IV.Length, cipher.Length);
                return Convert.ToBase64String(output);
            }
        }

        public static string DesencriptarReversible(string base64IvYCifrado)
        {
            EnsureAesKey();

            if (string.IsNullOrEmpty(base64IvYCifrado)) return string.Empty;

            byte[] combined = Convert.FromBase64String(base64IvYCifrado);

            using (var aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                int blockBytes = aes.BlockSize / 8;
                if (combined.Length < blockBytes + 1)
                    throw new ArgumentException("Entrada inválida (muy corta).", nameof(base64IvYCifrado));

                var iv = new byte[blockBytes];
                var cipher = new byte[combined.Length - blockBytes];
                Buffer.BlockCopy(combined, 0, iv, 0, blockBytes);
                Buffer.BlockCopy(combined, blockBytes, cipher, 0, cipher.Length);
                aes.IV = iv;

                using (var dec = aes.CreateDecryptor())
                {
                    var plain = dec.TransformFinalBlock(cipher, 0, cipher.Length);
                    return Encoding.UTF8.GetString(plain);
                }
            }
        }

        public static string EncriptarIrreversible(string texto)
        {
            if (texto == null) texto = string.Empty;

            var salt = new byte[PasswordSaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(texto, salt, PasswordIterations))
            {
                var key = pbkdf2.GetBytes(PasswordKeySize);
                return Convert.ToBase64String(salt) + ":" +
                       Convert.ToBase64String(key) + ":" +
                       PasswordIterations.ToString();
            }
        }

        public static bool VerificarIrreversible(string texto, string almacenado)
        {
            if (string.IsNullOrEmpty(almacenado)) return false;

            var parts = almacenado.Split(':');
            if (parts.Length != 3) return false;

            try
            {
                var salt = Convert.FromBase64String(parts[0]);
                var hash = Convert.FromBase64String(parts[1]);
                var iterations = int.Parse(parts[2]);

                using (var pbkdf2 = new Rfc2898DeriveBytes(texto ?? string.Empty, salt, iterations))
                {
                    var test = pbkdf2.GetBytes(hash.Length);
                    if (test.Length != hash.Length) return false;

                    int diff = 0;
                    for (int i = 0; i < hash.Length; i++)
                        diff |= test[i] ^ hash[i];
                    return diff == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void EnsureAesKey()
        {
            if (_aesKey != null) return;
            SetEncryptionKeyFromPassphrase_Default();
        }

        private static void SetEncryptionKeyFromPassphrase_Default()
        {
            var salt = Encoding.UTF8.GetBytes(AesSaltText);
            using (var pbkdf2 = new Rfc2898DeriveBytes(DefaultPassphrase, salt, AesDeriveIterations))
            {
                var key = pbkdf2.GetBytes(32); // 256-bit
                _aesKey = new byte[32];
                Buffer.BlockCopy(key, 0, _aesKey, 0, 32);
            }
        }

        private static string ComputeSha256Hex(string text)
        {
            if (text == null) text = string.Empty;
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));
                return sb.ToString();
            }
        }

        private static string GenerateSecureRandomString(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            const string Allowed = "#$%*abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var pool = Allowed.ToCharArray();
            var result = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < length; i++)
                    result[i] = pool[NextInt32(rng, pool.Length)];
            }
            return new string(result);
        }

        private static int NextInt32(RandomNumberGenerator rng, int maxExclusive)
        {
            if (rng == null) throw new ArgumentNullException(nameof(rng));
            if (maxExclusive <= 0) throw new ArgumentOutOfRangeException(nameof(maxExclusive));

            uint limit = (uint.MaxValue / (uint)maxExclusive) * (uint)maxExclusive;
            var bytes = new byte[4];
            uint value;
            do
            {
                rng.GetBytes(bytes);
                value = BitConverter.ToUInt32(bytes, 0);
            } while (value >= limit);

            return (int)(value % (uint)maxExclusive);
        }
    }
}
