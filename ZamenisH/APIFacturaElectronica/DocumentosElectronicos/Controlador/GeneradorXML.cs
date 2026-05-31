using Domain;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace DocumentosElectronicos.Controlador
{
    public static class GeneradorXML
    {
        public static List<FacturasR> rTemp;
        public static List<RCCAJA> rTempCaja;
        public static string DocPrestadorTemp, PrefElectronTemp, NumElectronTemp;

        public static decimal ConvertirValor(decimal? valor)
        {
            return Math.Round(valor ?? 0m, 2);
        }      
    }
}
