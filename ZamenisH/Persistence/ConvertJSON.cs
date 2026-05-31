using System.Text;

namespace ZamenisHealth.Clases
{
    public static class ConvertJSON
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int Base = 62;        

        public static string EncodeBase62(long num)
        {
            if (num == 0) return Alphabet[0].ToString();

            var sb = new StringBuilder();
            while (num > 0)
            {
                sb.Insert(0, Alphabet[(int)(num % Base)]);
                num /= Base;
            }
            return sb.ToString();
        }

        public static long DecodeBase62(string str)
        {
            long num = 0;
            foreach (char c in str)
            {
                num = num * Base + Alphabet.IndexOf(c);
            }
            return num;
        }
    }  
}
