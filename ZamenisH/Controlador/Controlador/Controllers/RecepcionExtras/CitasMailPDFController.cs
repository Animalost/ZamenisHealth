using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class CitasMailPDFController
    {
        private AgendaService agendaService;
        private EmailService emailService;
        private PacientesService pacientesService;

        public CitasMailPDFController()
        {
            agendaService = new AgendaService();
            emailService = new EmailService();
            pacientesService = new PacientesService();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }

        public List<CXN_EMAIL> getAllEmails()
        {
            return emailService.getAllEmails().GetAwaiter().GetResult();
        }

        public bool Actualiza_Email(int pac, string email)
        {
            return pacientesService.Actualiza_Email(pac, email).GetAwaiter().GetResult();
        }
    }
}
