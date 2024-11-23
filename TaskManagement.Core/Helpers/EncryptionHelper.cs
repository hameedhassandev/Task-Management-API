using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly string Key = "eXeGymFYP87GAcgag7J72nLPIsY7h6yUChnobI1JcGA=";
        public static string Encrypt(string plainText)
        {
            try
            {
                byte[] key = Convert.FromBase64String(Key);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var memoryStream = new MemoryStream())
                    {
                        // Write IV to the output stream first
                        memoryStream.Write(aes.IV, 0, aes.IV.Length);

                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                //log
                return string.Empty;
            }
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                byte[] key = Convert.FromBase64String(Key);
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // Extract IV from the ciphertext
                    byte[] iv = new byte[16];
                    Array.Copy(buffer, 0, iv, 0, iv.Length);

                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var memoryStream = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {
                //log
                return string.Empty;
            }

        }
    }
}
