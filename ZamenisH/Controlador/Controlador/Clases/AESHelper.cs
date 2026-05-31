using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Controlador.Clases
{
    public class AESHelper
    {
        public static string Key;  // Debe ser la misma en API y Cliente
        public static string IV;

        public static string GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return Convert.ToBase64String(aes.Key);
            }
        }

        public static string GenerateIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return Convert.ToBase64String(aes.IV);
            }
        }

        public static void setKeyIV(string _key, string _iv)
        {
            Key = _key;
            IV = _iv;
        }

        public static string Encrypt(string plainText)
        {
            byte[] keyBytes = Convert.FromBase64String(Key);
            byte[] ivBytes = Convert.FromBase64String(IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();  // <<--- Asegúrate de llamar a esto en el cifrado
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }


    }
}
