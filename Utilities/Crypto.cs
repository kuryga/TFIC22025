using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utilities
{
    public static class Crypto
    {
        private static string publickey = "PUBLICABC123";
        private static string secretkey = "SECRETCBA321";

        // Cache allowed chars as an array once
        private const string AllowedChars = "#$%*abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly char[] AllowedCharsArray = AllowedChars.ToCharArray();

        public static string HashSha256(string toHash)
        {
            var sb = new StringBuilder();
            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(toHash ?? string.Empty));
                for (int i = 0; i < result.Length; i++)
                    sb.Append(result[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string Encript(string toEncript)
        {
            try
            {
                string ToReturn;
                // DES requires 8-byte Key and IV (64-bit). Truncate/pad to 8 bytes.
                byte[] key = Take8(Encoding.UTF8.GetBytes(publickey));
                byte[] iv = Take8(Encoding.UTF8.GetBytes(secretkey));

                byte[] input = Encoding.UTF8.GetBytes(toEncript ?? string.Empty);

                using (var des = new DESCryptoServiceProvider())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public static string Decript(string toDecript)
        {
            try
            {
                string ToReturn;
                byte[] key = Take8(Encoding.UTF8.GetBytes(publickey));
                byte[] iv = Take8(Encoding.UTF8.GetBytes(secretkey));

                // normalize spaces to '+' in base64 strings
                string normalized = (toDecript ?? string.Empty).Replace(" ", "+");
                byte[] input = Convert.FromBase64String(normalized);

                using (var des = new DESCryptoServiceProvider())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Encoding.UTF8.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public static string RandomString(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length");

            var result = new char[length];
            int n = AllowedCharsArray.Length;

            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < length; i++)
                {
                    int idx = NextInt32(rng, n); // uniform 0..n-1
                    result[i] = AllowedCharsArray[idx];
                }
            }

            return new string(result);
        }

        // -------- helpers --------

        // Uniform integer in [0, maxExclusive) using rejection sampling, works on older TFMs
        private static int NextInt32(RandomNumberGenerator rng, int maxExclusive)
        {
            if (rng == null) throw new ArgumentNullException("rng");
            if (maxExclusive <= 0) throw new ArgumentOutOfRangeException("maxExclusive");

            // We generate a UInt32 and reject values that would skew the distribution.
            // 2^32 % maxExclusive gives the "bias" region.
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

        // Ensure exactly 8 bytes (DES key/IV) by truncating or padding zeros
        private static byte[] Take8(byte[] source)
        {
            var dest = new byte[8];
            if (source != null && source.Length > 0)
            {
                int len = Math.Min(8, source.Length);
                Buffer.BlockCopy(source, 0, dest, 0, len);
            }
            return dest;
        }
    }
}
