using System;
using System.Collections.Generic;

namespace Persistence.CXN_ADJUNTOS.Interfaces
{
    public interface IImagenA
    {
        List<byte[]> getImagenes(DateTime Desde, DateTime Hasta, string Carpeta);
        byte[] getPDF(DateTime Desde, DateTime Hasta, string Carpeta);
        List<string> getImagenesCarpetas(DateTime Desde, DateTime Hasta);
        List<string> getImagenesCarpetasPDF(DateTime Desde, DateTime Hasta);
    }
}
