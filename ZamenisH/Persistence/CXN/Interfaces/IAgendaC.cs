using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IAgendaC
    {
        bool insert016(int pacid, DateTime fecha);
        string getNameServicioFHIR(int CodeServ);
        void CancelacionInterna(string Razon, string User, int HorId, string Motivo);
        Dictionary<string, string> SugerenciaServicio(int Paciente);
        //string Calcular(int Hoy, int Paciente, string TipoServicio);
        string Calcular2(int Paciente, string TipoServicio);
        string Calcular3(int Paciente, string TipoServicio);
        void setRecepcion(string _valor);
        List<CXN_HORARIO> ListarCitasXPaciente(int PacId, DateTime fecha);
        string Extract(string input, int len, int ini);
        string Festivo(DateTime fecha);
        int ConsultarFecha(DateTime _fecha, int IdProf);
        otrosDatosPacienteHorario cargarAdmision(int Admision, string filter);
        bool addAutroizacion(int horid, string autroizacion, int Cantidad);
        bool addValidacionPin(int horid, string validaPin);
        bool addRegAtn(int Admision, string Reg);
        int getIdPacByAdmition(int Admition, string Estado);
        (string TipoId, string IdNum) getIdbyAdmision(int Admision);
        List<DiasAgenda> getCalendario();
        List<CXN_DIAS_WEB> CargarList(int Prof);
        bool Grabar(CXN_DIAS_WEB dias);
        bool Desbloquear(int Position, string User);
        Dictionary<string, string> ConsultaDatosAutorizacionMedGen(int Paciente, DateTime FechaCita);
        string consultarCitasMismoDia(int Paciente, int Bodega, DateTime Fecha);
        List<string> CargarRazones();
        (DateTime limite, string observa, string tiposerv, string text) searchAdmition(int Admision);
        string CrearHistorias(int Adm, string user);
        List<CXN_HORARIO> CargarGrilla(string Tipo);
        List<CXN_HORARIO> consultaCancelaWEB(string Documento);
        List<CXN_DIAS_WEB> CargarListBlocked(int Prof);
        bool EspacioRobado(int Cia, int Bod, string IdHora, DateTime Fecha);
        CXN_HORARIO getIdPacByAdmitionReportCitas(int Admition);
        int getOcupados(DateTime fecha, int Prof);
        void updateServicoFromFactura(int Admision, string CUP);
        (int Cantidad, string Clase) GenerarImprentaAutomatica(int Admision);
        List<CXN_HORARIO> ListarCitasXPaciente(int PacId, DateTime fecha, int Bodega);
        bool ConsultarNavyEnfermeria(int Paciente, int Bodega, DateTime Fecha);
        List<CXN_HORARIO> consultaCancelaWEB(DateTime Fecha);
        string UltimaAdmisionValidaVales(int Admision, int Paciente, DateTime FechaActual);
        (string Autorizacion, string Cantidad, string Estado) CitaMismoDiaGetAutorizacion(int PacId, DateTime fecha, int Bodega);
    }
}
