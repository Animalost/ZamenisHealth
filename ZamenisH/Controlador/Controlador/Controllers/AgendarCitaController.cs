using Controlador.Services;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class AgendarCitaController
    {
        private AseguradoraService aseguradoraService;
        private AgendarCitaService agendarCitaService;
        private PacientesService pacientesService;
        private ConveniosService conveniosService;
        private ConfSystemService confSystemService;
        private HCMGService hCMGService;
        private BodegaService bodegaService;
        private AgendaService agendaService;

        public AgendarCitaController()
        {
            agendarCitaService = new AgendarCitaService();
            conveniosService = new ConveniosService();
            aseguradoraService = new AseguradoraService();
            bodegaService = new BodegaService();
            confSystemService = new ConfSystemService();
            hCMGService = new HCMGService();
            agendaService = new AgendaService();
            pacientesService = new PacientesService();
        }

        public List<string> ListaDocs()
        {
            return agendarCitaService.ListaDocs().GetAwaiter().GetResult();
        }
        public List<string> ListaRegimen()
        {
            return agendarCitaService.ListaRegimen().GetAwaiter().GetResult();
        }
        public List<string> CargarServicios(string tbodega, int nbodega)
        {
            return conveniosService.CargarServicios(tbodega, nbodega).GetAwaiter().GetResult();
        }
        public List<CXN_ASEGURADORA> GetAseguradoras()
        {
            return aseguradoraService.GetAseguradoras().GetAwaiter().GetResult();
        }
        public CXN_BODEGAS getDataBodega(string NameBodega)
        {
            return bodegaService.getDataBodega(NameBodega).GetAwaiter().GetResult();
        }
        public CXN_BODEGAS GetDataBodegaCode(int CodeBodega)
        {
            return bodegaService.GetDataBodegaCode(CodeBodega).GetAwaiter().GetResult();
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
        public string Calcular2(string paciente, string tipo, string inicio)
        {
            return agendarCitaService.Calcular2(paciente, tipo, inicio).GetAwaiter().GetResult();
        }
        public Dictionary<string, string> SugerenciaServicio(string paciente)
        {
            return hCMGService.SugerenciaServicio(paciente).GetAwaiter().GetResult();
        }
        public Dictionary<string, string> getListado()
        {
            return confSystemService.getListado().GetAwaiter().GetResult();
        }
        public List<CXN_HORARIO> CitasProximas(int paciente, DateTime fecha)
        {
            return agendarCitaService.CitasProximas(paciente, fecha).GetAwaiter().GetResult();
        }
        public List<CXN_HORARIO> CargarPrevios(int paciente)
        {
            return agendarCitaService.CargarPrevios(paciente).GetAwaiter().GetResult();
        }
        public string Observacioprevia(int admision)
        {
            return agendarCitaService.Observacioprevia(admision).GetAwaiter().GetResult();
        }
        public List<CXN_HORARIO> ListarCitasXPaciente(int idpaciente, DateTime fecha)
        {
            return agendarCitaService.ListarCitasXPaciente(idpaciente, fecha).GetAwaiter().GetResult(); 
        }
        public bool EspacioRobado(int Cia, int Bod, string IdHora, DateTime Fecha)
        {
            return agendarCitaService.EspacioRobado(Cia, Bod, IdHora, Fecha).GetAwaiter().GetResult();
        }
        public CXN_ASEGURADORA GetInfoFromAsebyName(string nameAse)
        {
            return aseguradoraService.GetInfoFromAsebyName(nameAse).GetAwaiter().GetResult();
        }
        public bool ActualizarPaciente(CXN_PACIENTES P)
        {
            return agendarCitaService.ActualizarPaciente(P).GetAwaiter().GetResult();
        }
        public CXN_CONVENIOS ServicioCUP(int Ase, string Service)
        {
            return conveniosService.ServicioCUP(Ase, Service).GetAwaiter().GetResult(); 
        }
        public int CrearCitaMedica(CXN_HORARIO request)
        {
            return agendaService.CrearCitaMedica(request).GetAwaiter().GetResult();
        }
        public bool updateObservaTemp(string ObTemp, int Admision)
        {
            return agendarCitaService.updateObservaTemp(ObTemp, Admision).GetAwaiter().GetResult(); 
        }
    }
}
