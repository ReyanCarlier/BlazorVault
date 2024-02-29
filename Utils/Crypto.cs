using System.Security.Cryptography;
using System.Text;

namespace BlazorVault.Utils
{
    /**
     *  Crypto class for hashing, encrypting, and decrypting data.
     *  Mostly used for passwords.
     */
    internal class Crypto
    {
        public static bool IsPasswordStrongEnough(string password)
        {
            if (password.Length < 12)
                return false;

            bool hasCaps = false;
            bool hasNumber = false;
            bool hasSymbol = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasCaps = true;
                else if (char.IsDigit(c))
                    hasNumber = true;
                else if (char.IsSymbol(c) || char.IsPunctuation(c))
                    hasSymbol = true;
                if (hasCaps && hasNumber && hasSymbol)
                    break;
            }
            return hasCaps && hasNumber && hasSymbol;
        }

        public static string RandomPasswordGenerator(int length, bool includeSpecialCharacters = true)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            if (includeSpecialCharacters)
                chars += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            StringBuilder stringBuilder = new();
            Random random = new();
            for (int i = 0; i < length; i++)
                _ = stringBuilder.Append(chars[random.Next(chars.Length)]);
            return stringBuilder.ToString();
        }

        public static string Hash(string input)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static string Encrypt(string input, string key)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            key = Hash(key)[..32];

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string input, string key)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            key = Hash(key)[..32];

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
