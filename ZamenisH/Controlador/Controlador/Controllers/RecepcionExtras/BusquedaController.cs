using Controlador.Services;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class BusquedaController
    {
        private PacientesService pacientesService;
        private AgendarCitaService agendarCitaService;
        private AgendaService agendaService;
        private CompañiaService compañiaService;
        private BodegaService bodegaService;

        public BusquedaController()
        {
            pacientesService = new PacientesService();
            agendarCitaService = new AgendarCitaService();
            agendarCitaService = new AgendarCitaService();
            compañiaService = new CompañiaService();
            bodegaService = new BodegaService();
        }

        public CXN_PACIENTES LlamarPacienteOnlyDOC(string numid)
        {
            return pacientesService.LlamarPacienteOnlyDOC(numid).GetAwaiter().GetResult();
        }

        public List<CXN_HORARIO> ListarCitasXPaciente(int idpaciente, DateTime fecha)
        {
            return agendarCitaService.ListarCitasXPaciente(idpaciente, fecha).GetAwaiter().GetResult();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }

        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return compañiaService.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();
        }

        public CXN_BODEGAS getDataBodega(string NameBodega)
        {
            return bodegaService.getDataBodega(NameBodega).GetAwaiter().GetResult();    
        }
    }
}
