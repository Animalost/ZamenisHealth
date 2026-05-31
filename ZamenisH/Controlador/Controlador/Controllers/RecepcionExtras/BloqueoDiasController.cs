using Controlador.Services;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class BloqueoDiasController
    {
        private BodegaService _bodegaService;
        private AgendaService _agendaService;
        private CalendarioService _calendarioService;

        public BloqueoDiasController()
        {
            _bodegaService = new BodegaService();
            _agendaService = new AgendaService();
            _calendarioService = new CalendarioService();
        }

        public List<string> ObtenerListaProfesionales(string Tipo)
        {
            return _bodegaService.ObtenerListaProfesionales(Tipo).GetAwaiter().GetResult();
        }

        public CXN_BODEGAS GetDataBodegaCode(string nameBodega)
        {
            return _bodegaService.getDataBodega(nameBodega).GetAwaiter().GetResult();
        }

        public List<CXN_DIAS_WEB> CargarList(int codeprof)
        {
            return _agendaService.CargarList(codeprof).GetAwaiter().GetResult();
        }

        public int ConsultarFecha(int _codeProfesional, DateTime fecha)
        {
            return _calendarioService.ConsultarFecha(_codeProfesional, fecha).GetAwaiter().GetResult();
        }

        public bool Grabar(CXN_DIAS_WEB dias)
        {
            return _calendarioService.Grabar(dias).GetAwaiter().GetResult();
        }

        public bool Desbloquear(int position, string user)
        {
            return _calendarioService.Desbloquear(position, user).GetAwaiter().GetResult();
        }
    }
}
