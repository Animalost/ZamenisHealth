using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class NovedadesAdmisionController
    {
        private AdmisionesService m_dadmisionesService;
        private AgendaService agendaService;

        public NovedadesAdmisionController()
        {
            m_dadmisionesService = new AdmisionesService();
            agendaService = new AgendaService();
        }

        public List<string> CargarRazones()
        {
            return m_dadmisionesService.CargarRazones().GetAwaiter().GetResult();
        }

        public CXN_HORARIO DatosforMailSMS(int Admision)
        {
            return agendaService.DatosforMailSMS(Admision).GetAwaiter().GetResult();
        }

        public bool CancelacionInterna(string razon, string user, int horId, string motivo)
        {
            return agendaService.CancelacionInterna(razon, user, horId, motivo).GetAwaiter().GetResult();
        }

        public bool Inasistencia_Cita(string razon, int horId)
        {
            return agendaService.Inasistencia_Cita(razon, horId).GetAwaiter().GetResult();
        }

        public bool Retardo_Cita(string razon, int horId, string minutos)
        {
            return agendaService.Retardo_Cita(razon, horId, minutos).GetAwaiter().GetResult();
        }
    }
}
