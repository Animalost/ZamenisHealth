using Controlador.Services;

using Domain;
using Domain.CXN;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controlador.Controllers
{
    public class AdmisionesController
    {
        private readonly ConfSystemService confSystemService;
        private readonly AgendarCitaService agendarCitaService;
        private readonly AgendaService agendaService;
        private readonly ConveniosService conveniosService;
        private readonly AseguradoraService aseguradoraService;
        private readonly PacientesService pacientesService;
        private readonly AdmisionesService admisionesService;
        private readonly ZonasService zonasService;
        private readonly CompañiaService compañiaService;
        private readonly OMService oMService;

        public AdmisionesController() 
        {
            confSystemService = new ConfSystemService();
            agendaService = new AgendaService();
            agendarCitaService = new AgendarCitaService();
            conveniosService = new ConveniosService();
            aseguradoraService = new AseguradoraService();
            pacientesService = new PacientesService();
            zonasService = new ZonasService();
            admisionesService = new AdmisionesService();
            compañiaService = new CompañiaService();
            oMService = new OMService();
        }

        public Dictionary<string, string> getListado()
        {
            return confSystemService.getListado().GetAwaiter().GetResult();
        }
        public List<string> ListaDocs()
        {
            return agendarCitaService.ListaDocs().GetAwaiter().GetResult();
        }
        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }
        public List<string> CargarServicios(string tbodega, int nbodega)
        {
            return conveniosService.CargarServicios(tbodega, nbodega).GetAwaiter().GetResult();
        }
        public CXN_CONVENIOS ServicioNombre(string cup, int ase, string serv)
        {
            return conveniosService.ServicioNombre(cup, ase, serv).GetAwaiter().GetResult();
        }
        public List<string> CargarAseguradorasXServ(string tserv)
        {
            return aseguradoraService.CargarAseguradorasXServ(tserv).GetAwaiter().GetResult();
        }
        public List<string> ListaRegimen()
        {
            return agendarCitaService.ListaRegimen().GetAwaiter().GetResult();
        }
        public CXN_ASEGURADORA GetInfoFromAsebyCode(int codeaseguradora)
        {
            return aseguradoraService.GetInfoFromAsebyCode(codeaseguradora).GetAwaiter().GetResult();
        }
        public string Carga_Regimen(string coderegimen)
        {
            return pacientesService.Carga_Regimen(coderegimen).GetAwaiter().GetResult();
        }
        public string DepartamentoNombre(string zonCode)
        {
            return zonasService.DepartamentoNombre(zonCode).GetAwaiter().GetResult();
        }
        public string MunicipioNombre(string CodMun, string DepCod)
        {
            return zonasService.MunicipioNombre(CodMun, DepCod).GetAwaiter().GetResult();
        }
        public List<string> getListPaises()
        {
            return zonasService.getListPaises().GetAwaiter().GetResult();
        }
        public string getNamePais(string namePais)
        {
            return zonasService.getNamePais(namePais).GetAwaiter().GetResult(); 
        }
        public List<CXN_HORARIO> ListarCitasXPaciente(int pacid, int bodega, DateTime fecha)
        {
            return admisionesService.ListarCitasXPaciente(pacid, bodega, fecha).GetAwaiter().GetResult();
        }
        public Dictionary<string, string> ConsultaDatosAutorizacionMedGen(int pacid, DateTime fecha)
        {
            return agendaService.ConsultaDatosAutorizacionMedGen(pacid, fecha).GetAwaiter().GetResult();
        }
        public string Calcular2(string paciente, string tipo, string inicio)
        {
            return agendarCitaService.Calcular2(paciente, tipo, inicio).GetAwaiter().GetResult();
        }
        public CXN_ASEGURADORA GetInfoFromAsebyName(string nameAse)
        {
            return aseguradoraService.GetInfoFromAsebyName(nameAse).GetAwaiter().GetResult();
        }
        public string consultarCitasMismoDia(int pacid, int bodega, DateTime fecha)
        {
            return admisionesService.consultarCitasMismoDia(pacid, bodega, fecha).GetAwaiter().GetResult();
        }
        public CXN_CONVENIOS ServicioCUP(int Ase, string Service)
        {
            return conveniosService.ServicioCUP(Ase, Service).GetAwaiter().GetResult();
        }
        public int updateCitaAdmisionar(CXN_HORARIO H)
        {
            return admisionesService.updateCitaAdmisionar(H).GetAwaiter().GetResult();
        }
        public int PendientesChecked(int admision, string estado)
        {
            return admisionesService.PendientesChecked(admision ,estado).GetAwaiter().GetResult();
        }
        public int InicioControlCuraciones(int admision, string estado)
        {
            return admisionesService.InicioControlCuraciones(admision, estado).GetAwaiter().GetResult();
        }
        public bool consularMGMismoDia(int admision, DateTime fecha)
        {
            return admisionesService.consularMGMismoDia(admision, fecha).GetAwaiter().GetResult();
        }
        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return compañiaService.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();
        }
        public (int Cantidad, string Clase) GenerarImprentaAutomatica(int admision)
        {
            return admisionesService.GenerarImprentaAutomatica(admision).GetAwaiter().GetResult();
        }
        public List<FirmasR> Firmas_Print(int admision, FirmasR F, bool autocomplete)
        {
            return admisionesService.Firmas_Print(admision, F, autocomplete).GetAwaiter().GetResult();
        }

        public bool OPend(CXN_OPEND O)
        {
            return admisionesService.OPend(O).GetAwaiter().GetResult();
        }

        public bool consumirAutorizacion(int Paciente, string Autorizacion, string Estado)
        {
            return oMService.consumirAutorizacion(Paciente, Autorizacion, Estado).GetAwaiter().GetResult();
        }

        public string getCodePais(string code)
        {
            return zonasService.getCodePais(code).GetAwaiter().GetResult();
        }

        public bool Actualiza_Pac(CXN_PACIENTES P)
        {
            return pacientesService.Actualiza_Pac(P).GetAwaiter().GetResult();
        }

        public bool _updateAseHorario(int ase, string pac, int adm)
        {
            return admisionesService._updateAseHorario(ase, pac, adm).GetAwaiter().GetResult();
        }        
    }
}
