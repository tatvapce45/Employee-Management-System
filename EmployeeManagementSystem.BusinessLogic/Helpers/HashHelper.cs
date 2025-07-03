using System.Security.Cryptography;
using System.Text;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementSystem.BusinessLogic.Helpers
{
    public class HashHelper : IHashHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public HashHelper(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var keyString = configuration["HashGenerator:HashKey"];
            var ivString = configuration["HashGenerator:HashIV"];

            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(ivString))
            {
                throw new ArgumentException("Encryption key and IV must be provided in configuration.");
            }

            _key = Encoding.UTF8.GetBytes(keyString);
            _iv = Encoding.UTF8.GetBytes(ivString);
            ValidateKeyAndIV(_key, _iv);
        }

        public string Encrypt(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }


            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string? cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }

            try
            {
                var buffer = Convert.FromBase64String(cipherText);

                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(buffer);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs, Encoding.UTF8);

                return sr.ReadToEnd();
            }
            catch (FormatException)
            {
                return string.Empty;
            }
            catch (CryptographicException)
            {
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void ValidateKeyAndIV(byte[] key, byte[] iv)
        {
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            {
                throw new ArgumentException("Invalid key size. Key must be 16, 24, or 32 bytes.");
            }

            if (iv.Length != 16)
            {
                throw new ArgumentException("Invalid IV size. IV must be 16 bytes.");
            }
        }
    }
}
