using System;
using System.Text;
using System.Security.Cryptography;

namespace DAL.Seguridad
{
    public static class SecurityUtilities
    {
        private static byte[] _aesKey; // 32 bytes (AES-256)
        private const string DefaultPassphrase = "TFIUAI2025AAGK";         // TODO: mover a configuración
        private const string AesSaltText = "UrbanSoft-AES-Key-Salt";       // sal fija para derivar la clave
        private const int AesDeriveIterations = 100000;

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

                // IV determinístico derivado del texto plano
                aes.IV = DeriveDeterministicIV(textoPlano);

                byte[] plain = Encoding.UTF8.GetBytes(textoPlano);
                byte[] cipher;
                using (var enc = aes.CreateEncryptor())
                {
                    cipher = enc.TransformFinalBlock(plain, 0, plain.Length);
                }

                // Formato: IV || CIPHER (en Base64)
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

                int blockBytes = aes.BlockSize / 8; // 16 bytes para AES
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

            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(texto);
                var hash = md5.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerificarIrreversible(string texto, string hashMd5Base64)
        {
            if (string.IsNullOrWhiteSpace(hashMd5Base64)) return false;

            string calc = EncriptarIrreversible(texto ?? string.Empty);
            return ConstantTimeEquals(calc.Trim(), hashMd5Base64.Trim());
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

        private static byte[] DeriveDeterministicIV(string textoPlano)
        {
            const string Pepper = "UrbanSoft-AES-IV";
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes((textoPlano ?? string.Empty) + "|" + Pepper);
                var hash = sha.ComputeHash(bytes);
                var iv = new byte[16]; // 128 bits
                Buffer.BlockCopy(hash, 0, iv, 0, 16);
                return iv;
            }
        }

        private static bool ConstantTimeEquals(string a, string b)
        {
            if (a == null || b == null) return false;
            a = a.Trim();
            b = b.Trim();
            if (a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
