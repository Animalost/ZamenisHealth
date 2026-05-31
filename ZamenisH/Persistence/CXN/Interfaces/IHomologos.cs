using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Data;

namespace Persistence.CXN.Interfaces
{
    public interface IHomologos
    {
        List<CXN_FACTURA> Filtra_Generales(string Tipo, int Compañia, int Factura);
        bool ConsultarExistenciaGeneral(string Homologo, int Compañia);
        bool ConsultarExistenciaVentas(string Homologo, int Compañia);
        bool AddHomologoGeneral(string Homologo, int Pos, DateTime Fecha, DateTime Hora, string CUFE, string Resolucion);
        bool AddHomologoRecepcion(string Homologo, int Pos, DateTime Fecha, DateTime Hora, string CUFE, string Resolucion);
        bool ConsultarFactura(int FZ, string FC);
        bool Verifica_Homologo(string FH, string FC);
        bool Verifica_HomologoRec(string FH, string FC);
        bool ActualizarMasivo(string FH, int FZ, string FC, DateTime FF, string CUFE, DateTime HORA, string RESOLUCION);
        bool ActualizarMasivoRec(string FH, int FZ, string FC, DateTime FF, string CUFE, DateTime HORA, string RESOLUCION);
        DataView ImportarDatos(string nombrearchivo);
        bool ConsultarRecibo(int FZ, string FC);
        bool ActualizarMasivoRecibo(int Recibo, string Homologo, string CUFE, string Resolucion);
    }
}
