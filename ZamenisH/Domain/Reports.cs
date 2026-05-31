using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Domain
{
    public class FirmasR : CXN_CIA
    {
        public string EmpresaNombre { get; set; }
        public string EmpresaDireccion { get; set; }
        public string EmpresaTelefono { get; set; }
        public string Con_Nombre { get; set; }        
        public byte[] FirmaByte { get; set; }
        public int Admision { get; set; }
        public int Cantidad { get; set; }
    }

    public class GerencialR
    {
        public string Medico { get; set; }
        public string Asistidas { get; set; }
        public string Estado { get; set; }
        public string Bodega { get; set; }
        public string Service { get; set; }
        public DateTime Desde1 { get; set; }
        public DateTime Hasta1 { get; set; }
        public DateTime Fecha_Actual { get; set; }
    }

    public class ExportInExcel
    {
        public string Dato1 { get; set; }
        public string Dato2 { get; set; }
        public string Dato3 { get; set; }
        public string Dato4 { get; set; }
        public string Dato5 { get; set; }
        public string Dato6 { get; set; }
        public string Dato7 { get; set; }
        public string Dato8 { get; set; }
        public string Dato9 { get; set; }
        public string Dato10 { get; set; }
        public string Dato11 { get; set; }
        public string Dato12 { get; set; }
        public string Dato13 { get; set; }
        public string Dato14 { get; set; }
        public string Dato15 { get; set; }
        public string Dato16 { get; set; }
        public string Dato17 { get; set; }
        public string Dato18 { get; set; }
        public string Dato19 { get; set; }
        public string Dato20 { get; set; }
        public string Dato21 { get; set; }
        public string Dato22 { get; set; }
        public string Dato23 { get; set; }
        public string Dato24 { get; set; }
        public string Dato25 { get; set; }
        public string Dato26 { get; set; }
        public Dictionary<string, int> Dato27 = new Dictionary<string, int>();
    }
}
