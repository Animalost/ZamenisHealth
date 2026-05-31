using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.CXN.Interfaces
{
    public interface IFHIR
    {
        Task<List<CXN_HORARIO>> FiltrarEspecialidad(DateTime Fecha, string Especialidad);
        void InsertarEnvio_RDAAmbulatorio(CXN_RDA Respuesta);
        string Gender(string Tipo);
        List<string> GetConsumos();
        List<CXN_CONDICIONES> ConsultarAlergias(int Paciente);
        void ActualizarEnvio_RDACExterna(CXN_RDA Respuesta);
        string Modalidad(string code);
        string GrupoServicios(string Code);
        string GetCodeConsumo(string Name);
        string Etnia(string Name);
        string Discapacidad(string Name);
        bool InsertarToken(CXN_TOKENS_FHIR T);
        string RecuperarToken(int Prestador);
        CXN_TOKENS_FHIR RecuperarClaseToken(int Prestador);
        CXN_RDA ConsultarAdmision(int Admision);
        string GetCodeMedicamento(string Name);
        void InsertarEnvio_RDAPaciente(CXN_RDA Respuesta);
        void ActualizarEnvio_RDAPaciente(CXN_RDA Respuesta);
        string GrupoAlergias(string Code);
        List<CXN_CONDICIONES> ConsultarMedicamentos(int Paciente, DateTime Fecha);
        List<string> CargarRISK();
        string GetCodeRisk(string Name);
        string GetDesctecnoSalud(string Code);
        List<string> GetOcupaciones(string Ocupacion);
        string GetCodeOcupacion(string Name);
        CXN_HCMG GetNAMG(int Admition);
        void InsertarLOG_RDA(CXN_RDA_LOG Respuesta);
        List<CXN_RDA_LOG> GetLogs(DateTime Desde, DateTime Hasta);
        string GetLogFHIR(int Id);


        List<CXN_DATOS_FHIR> ListaDatosConfFHIR(int Prestador);
    }
}
