using Controlador.Services;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class MoverAgendaController
    {
        private AgendaService agendaService;
        private BodegaService bodegaService;


        public MoverAgendaController()
        {
            agendaService = new AgendaService();
            bodegaService = new BodegaService();
        }

        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }

        public CXN_BODEGAS getDataBodega(string NameBodega)
        {
            return bodegaService.getDataBodega(NameBodega).GetAwaiter().GetResult();
        }

        public bool changeProfesional(int admision,
                                      int bodega,
                                      string idHora,
                                      string observacion,
                                      DateTime fechaCita,
                                      DateTime hora)
        {
            return agendaService.changeProfesional(admision, bodega, idHora, observacion, fechaCita, hora).GetAwaiter().GetResult();  
        }

        public List<CXN_HORARIO> CargarAgenda(DateTime desde, int medico, int compañia, string dia)
        {
            return agendaService.CargarAgenda(desde, medico, compañia, dia).GetAwaiter().GetResult();
        }

        public List<string> ObtenerListaProfesionales(string Tipo)
        {
            return bodegaService.ObtenerListaProfesionales(Tipo).GetAwaiter().GetResult();
        }
    }
}
