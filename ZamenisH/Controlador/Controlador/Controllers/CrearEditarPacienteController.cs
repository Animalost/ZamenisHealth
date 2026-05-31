using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class CrearEditarPacienteController
    {
        private AgendarCitaService agendarCitaService;
        private AseguradoraService aseguradoraService;
        private PacientesService pacientesService;
        private ZonasService zonasService;

        public CrearEditarPacienteController()
        {
            agendarCitaService = new AgendarCitaService();
            aseguradoraService = new AseguradoraService();
            pacientesService = new PacientesService();
            zonasService = new ZonasService();
        }

        public List<string> ListaDocs()
        {
            return agendarCitaService.ListaDocs().GetAwaiter().GetResult();
        }

        public List<string> ListaRegimen()
        {
            return agendarCitaService.ListaRegimen().GetAwaiter().GetResult();
        }

        public List<CXN_ASEGURADORA> GetAseguradoras()
        {
            return aseguradoraService.GetAseguradoras().GetAwaiter().GetResult();   
        }

        public CXN_PACIENTES LlamarPacienteDOC(string tipoid, string numid)
        {
            return pacientesService.LlamarPacienteDOC(tipoid, numid).GetAwaiter().GetResult();  
        }

        public CXN_PACIENTES LlamarPacienteOnlyDOC(string numid)
        {
            return pacientesService.LlamarPacienteOnlyDOC(numid).GetAwaiter().GetResult();
        }

        public string Carga_Regimen(string coderegimen)
        {
            return pacientesService.Carga_Regimen(coderegimen).GetAwaiter().GetResult();    
        }

        public CXN_ASEGURADORA GetInfoFromAsebyCode(int codeaseguradora)
        {
            return aseguradoraService.GetInfoFromAsebyCode(codeaseguradora).GetAwaiter().GetResult();
        }

        public string DepartamentoNombre(string zonCode)
        {
            return zonasService.DepartamentoNombre(zonCode).GetAwaiter().GetResult();   
        }

        public string MunicipioNombre(string CodMun, string DepCod)
        {
            return zonasService.MunicipioNombre(CodMun, DepCod).GetAwaiter().GetResult();
        }

        public CXN_ASEGURADORA GetInfoFromAsebyName(string nameAse)
        {
            return aseguradoraService.GetInfoFromAsebyName(nameAse).GetAwaiter().GetResult();   
        }

        public string MunicipioCodigo(string NomMun, string NomDep)
        {
            return zonasService.MunicipioCodigo(NomMun, NomDep).GetAwaiter().GetResult();
        }

        public string DepartamentoCodigo(string NomDep)
        {
            return zonasService.DepartamentoCodigo(NomDep).GetAwaiter().GetResult();    
        }

        public bool Crea_Paciente(CXN_PACIENTES P)
        {
            return pacientesService.Crea_Paciente(P).GetAwaiter().GetResult();
        }

        public bool Existente(string document, int idpac)
        {
            return pacientesService.Existente(document, idpac).GetAwaiter().GetResult();
        }

        public bool Edita_Paciente(CXN_PACIENTES P)
        {
            return pacientesService.Edita_Paciente(P).GetAwaiter().GetResult();
        }
    }
}
