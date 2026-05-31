using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class RcCajaController
    {
        private readonly AgendaService agendaService;
        private readonly PacientesService pacientesService;
        private readonly RcCajaService rcCajaService;
        private readonly ReportesService reportesService;

        public RcCajaController()
        {
            agendaService = new AgendaService();
            pacientesService = new PacientesService();
            rcCajaService = new RcCajaService();
            reportesService = new ReportesService();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }

        public CXN_PACIENTES LlamarPacientebyId(int pacid)
        {
            return pacientesService.LlamarPacientebyId(pacid).GetAwaiter().GetResult();
        }

        public bool updateReciboHorario(int valor, int admision, string conRecaudo, string docFE)
        {
            return rcCajaService.updateReciboHorario(valor, admision, conRecaudo, docFE).GetAwaiter().GetResult();
        }

        public bool AgregarRecibo(CXN_RC_CAJA R)
        {
            return rcCajaService.AgregarRecibo(R).GetAwaiter().GetResult();
        }

        public bool Actualiza_Email(int pac, string email)
        {
            return pacientesService.Actualiza_Email(pac, email).GetAwaiter().GetResult();
        }

        public List<RCCAJA> ReciboRpt(int admision)
        {
            return reportesService.ReciboRpt(admision).GetAwaiter().GetResult();
        }

        public int getValor(string categoria)
        {
            return pacientesService.getValor(categoria).GetAwaiter().GetResult();
        }
    }
}
