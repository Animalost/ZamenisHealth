using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZamenisHealth.Clases
{
    public static class CodificadorVC
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "0123456789";
        private const string Symbols = "!@#$%^&*()_+-=<>?/[]{}|,.";

        // Codificación para símbolos - puedes ajustar esto según tus necesidades
        private static readonly string SymbolEncoding = "abcdefghijklmnopqrstuvwxyz";

        public static string Encrypt(string Password)
        {
            try
            {
                string textToEncrypt = Password;
                string ToReturn = "";
                string publickey = "12345678";
                string secretkey = "87654321";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
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

        public static string Encode(string input, int shift)
        {
            return Transform(input, shift);
        }

        public static string Decode(string input, int shift)
        {
            // Invertir el desplazamiento para decodificar
            return Transform(input, -shift);
        }

        static string Transform(string input, int shift)
        {
            var transformedChars = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.IsLetter(c))
                {
                    bool isUpper = char.IsUpper(c);
                    char baseChar = isUpper ? 'A' : 'a';
                    int alphabetIndex = (c - baseChar + shift) % 26;
                    if (alphabetIndex < 0)
                    {
                        alphabetIndex += 26;
                    }
                    transformedChars.Append((char)(baseChar + alphabetIndex));
                }
                else if (char.IsDigit(c))
                {
                    int numberIndex = (c - '0' + shift) % 10;
                    if (numberIndex < 0)
                    {
                        numberIndex += 10;
                    }
                    transformedChars.Append((char)('0' + numberIndex));
                }
                else if (Symbols.IndexOf(c) >= 0)
                {
                    int symbolIndex = Symbols.IndexOf(c);
                    int encodedIndex = symbolIndex % SymbolEncoding.Length;
                    transformedChars.Append(SymbolEncoding[encodedIndex]);
                }
                else
                {
                    throw new ArgumentException("Input contains invalid characters.");
                }
            }

            return transformedChars.ToString();
        }

        public static string MaskEmail(string email)
        {
            // Verificar si el correo tiene un formato válido
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Correo no válido");

            // Separar el correo en la parte local y el dominio
            string[] parts = email.Split('@');
            string localPart = parts[0];
            string domainPart = parts[1];

            // Reemplazar desde la mitad de la parte local con asteriscos
            int visibleChars = Math.Max(1, localPart.Length / 2);
            string maskedLocalPart = localPart.Substring(0, visibleChars) + new string('*', localPart.Length - visibleChars);

            // Reconstruir el correo
            return maskedLocalPart + "@" + domainPart;
        }
    }
}
