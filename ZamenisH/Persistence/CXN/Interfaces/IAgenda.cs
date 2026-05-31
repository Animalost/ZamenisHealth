using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IAgenda
    {
        //STORED PROCCEDURE
        List<CXN_HORARIO> CargarAgenda(int Medico, int Compañia, DateTime Desde, string Dia);

        //BASE 
        void Images_Adress();
        List<CXN_HORARIO> Asistencia(string ID);
        List<CXN_HORARIO> AsistenciaLastAut(string ID);
        string SearchCuracionForMG(int Paciente, DateTime Fecha, int Medico);
        List<CXN_HORARIO> CargarPrevios(int PacId);
        List<CXN_HORARIO> CitasProximas(int PacId, DateTime Fecha);
        int AgendarPaciente(CXN_HORARIO horario);
        string Observacioprevia(int Admision);
        CXN_HORARIO DatosforMailSMS(int Admision);
        void desbloquearEspacio(string Usuario, int Admision);
        string consularAdmisionEstado(int Admision);
        void anularAdmision(int Admision, string UserLogged);
        List<CXN_CIA> certificadoAsistencia(int Admision, string Texto);
        Dictionary<string, string> SugerenciaServicio(int Paciente);
        bool changeProfesional(int Admision, int Bodega, string IdHora, string Observacion, DateTime fechaCita, DateTime hora);
        void _updateAseHorario(int Ase, string Pac, int Adm);
        void ActualizaAdmision(string Cup, int Adm);
        bool updateCitaAdmisionar(CXN_HORARIO H);
        void CancelacionInterna(string Razon, string User, int HorId, string Motivo);
        void Inasistencia_Cita(int horid, string razon);
        void Retardo_Cita(string Minutos, string Razon, int horid);
        void addObservation(int Admision, string Observacion);
        void deleteHistoria(int Admision, string TipServ);
        void addFestivo(DateTime Fecha, string Motivo);
        void ConsumirAdmision(int Admition);
        void Graba_Hora_Atencion(int Atention);
        CXN_HORARIO getLastHorToCopy(int Admision);
        int getLastIDToCopy(int Paciente);
        void InicioControlCuraciones(int Admision, string estado);
        int consularMGMismoDia(int Pacientes, DateTime Fecha);
        void OPend(CXN_OPEND OP);
        bool getPendientes();
        List<CXN_HORARIO> getListPendientes();
        void ActualizarAutorizacionMG(int Admision, string Autoriza, int Cantidad);
        void OPendUpdate(CXN_OPEND O);
        void PendientesChecked(int Adm, string Estado);
        void OpenAdmition(int Admision, string Estado);
        bool OpenAdmition(int Admision);
        int AgendarPacienteJuntas(CXN_HORARIO horario);
        void Graba_Hora_Salida(int Atention);
        void updateAseguradoraFromCargo(int Asegura, int Admision);
        void updateObservaTemp(string ObTemp, int Admision);
        void updateTipoCitaMG(string Tipo, int Admision, string IniSesion);
        Dictionary<int, string> getSaleConsultas(DateTime Fecha, string Tipo);
        void updateSALIERONCONSULTA(int Admition);
        bool addSALECONSULTA(int Admision);
    }
}
