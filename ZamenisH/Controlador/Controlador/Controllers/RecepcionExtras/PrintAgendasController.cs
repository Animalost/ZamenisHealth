using Controlador.Services;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;

namespace Controlador.Controllers.RecepcionExtras
{
    public class PrintAgendasController
    {
        private readonly AgendarCitaService agendaService;
        private readonly BodegaService bodegaService;
        private readonly ReportesService reportesService;

        public PrintAgendasController()
        {
            agendaService = new AgendarCitaService();
            bodegaService = new BodegaService();
            reportesService = new ReportesService();
        }

        public List<string> ListaDocs()
        {
            return agendaService.ListaDocs().GetAwaiter().GetResult();
        }

        public DataTable Profesionales2(string tprofesional)
        {
            return bodegaService.Profesionales2(tprofesional).GetAwaiter().GetResult();
        }

        public List<MReportes.PrntAgendas> Genera_CitasXPac_Report(string doc, DateTime desde, DateTime hasta)
        {
            return reportesService.Genera_CitasXPac_Report(doc, desde, hasta).GetAwaiter().GetResult(); 
        }

        public List<MReportes.PrntAgendas> Genera_Export_Citas(string tipo_reporte_citas,
                                                                           DateTime fechaAgenda,
                                                                           string prof_med_citas)
        {
            return reportesService.Genera_Export_Citas(tipo_reporte_citas, fechaAgenda, prof_med_citas).GetAwaiter().GetResult();
        }
    }
}
