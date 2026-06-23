using Domain;
using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.CXN.Interfaces
{
    public interface ICargos
    {
        string insertService(CXN_CARGOS c);
        CXN_CARGOS GetCargosFHIR(int Admision);
        List<CotizacionR> GenerarDocumento(int Docs, int Cia);
        ListaCarga Carga_Plantilla();
        List<CXN_CARGOS> cotizacionesPrevias(string TID, string ID);
        void Rpt_Car_Tot(DateTime Desde, DateTime Hasta);
        void Total(DateTime Desde, DateTime Hasta);
        void Individual(DateTime Desde, DateTime Hasta, string Doc);
        bool InsertarCargoHistorias(CXN_CARGOS c);
        void deleteHistoria(int Admision);
        CXN_HORARIO BuscarCargo(int Admision);
        Task<List<CXN_HORARIO>> ListaCargos(DateTime fecha, bool Hecho);
        List<CXN_CARGOS> CargosAnt(int Admi);
        bool deleteCargo(int Admision);
        ListaProd DatoProd(int Ase, string Cod);
        Task<int> SaveCargo(CXN_CARGOS C);
        CXN_CARGOS getValores(int Posision);
        bool updateValores(CXN_CARGOS C);
        bool updateCia(int Cia, int Posision);
        bool updateAse(int Ase, int Posision);
        bool updateDate(DateTime Fecha, int Posision);
        bool HabilitaInhabilita(string Est, int Posision);
        bool updateCargos(CXN_CARGOS C);
        List<CXN_CARGOS> BuscarCargoTotal(int Admision);
        CXN_CARGOS getLasCargoToCopy(int Admision);
        List<CXN_CARGOS> getCargosPrevios(int Paciente);
        CXN_CARGOS getRIPS(int Admision);
        bool cargoExiste(int Admision);
        bool updateCargosMasivo(CXN_CARGOS C);
    }
}
