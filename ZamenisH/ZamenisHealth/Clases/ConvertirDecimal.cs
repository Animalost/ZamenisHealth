using System;

namespace ZamenisHealth.Clases
{
    public static class ConvertirDecimal
    {
        public static decimal ConvertirValor(decimal? valor)
        {
            return Math.Round(valor ?? 0m, 2);
        }
    }
}
