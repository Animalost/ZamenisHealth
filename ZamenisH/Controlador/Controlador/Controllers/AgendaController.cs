using Controlador.Services;
using Domain;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class AgendaController
    {
        private readonly BodegaService _bodegaService;
        private readonly CompañiaService _compañiaService;
        private readonly AgendaService _agendaService;
        private readonly ConfSystemService _confSystemService;
        private readonly CalendarioService _calendarioService;
        private readonly DisponibilidadService _disponibilidadService;
        private readonly ConveniosService _conveniosService;
        private readonly PacientesService _pacientesService;
        private readonly RcCajaService _rcrCajaService;
        private readonly LogService _logService;
        private readonly AdmisionesService _admisionesService;

        public AgendaController()
        {
            _bodegaService = new BodegaService();
            _compañiaService = new CompañiaService();
            _confSystemService = new ConfSystemService();
            _agendaService = new AgendaService();
            _calendarioService = new CalendarioService();
            _calendarioService = new CalendarioService();
            _disponibilidadService = new DisponibilidadService();
            _conveniosService = new ConveniosService();
            _pacientesService = new PacientesService();
            _rcrCajaService = new RcCajaService();
            _logService = new LogService();
            _admisionesService = new AdmisionesService();
        }

        public List<string> ObtenerListaProfesionales(string Tipo)
        {
            return _bodegaService.ObtenerListaProfesionales(Tipo).GetAwaiter().GetResult();
        }
        public List<string> ObtenerListaPrestadores()
        {
            return _compañiaService.ObtenerListaPrestadores().GetAwaiter().GetResult();
        }
        public Dictionary<string, string> getListado()
        {
            return _confSystemService.getListado().GetAwaiter().GetResult();
        }
        public CXN_BODEGAS getDataBodega(string NameBodega)
        {
            return _bodegaService.getDataBodega(NameBodega).GetAwaiter().GetResult();
        }
        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return _compañiaService.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();
        }
        public List<DiasAgenda> FechasFestivas()
        {
            return _calendarioService.FechasFestivas().GetAwaiter().GetResult();
        }
        public List<CXN_DIAS_WEB> CargarListBlocked(int _codeProfesional)
        {
            return _calendarioService.CargarListBlocked(_codeProfesional).GetAwaiter().GetResult();
        }
        public List<CXN_DISPONIBILIDAD_2> HorariosHabilitados(int bodega, string dia)
        {
            return _disponibilidadService.HorariosHabilitados(bodega, dia).GetAwaiter().GetResult();
        }
        public int HorariosOcupados(int bodega, DateTime fecha)
        {
            return _disponibilidadService.HorariosOcupados(bodega, fecha).GetAwaiter().GetResult();
        }
        public string Festivos(DateTime fecha)
        {
            return _disponibilidadService.Festivos(fecha).GetAwaiter().GetResult();
        }
        public List<CXN_HORARIO> CargarAgenda(DateTime desde, int medico, int compañia, string dia)
        {
            return _agendaService.CargarAgenda(desde, medico, compañia, dia).GetAwaiter().GetResult();
        }
        public string NameServiceCUP(string nameservicio)
        {
            return _conveniosService.NameServiceCUP(nameservicio).GetAwaiter().GetResult();
        }
        public string consularAdmisionEstado(int Admision)
        {
            return _agendaService.consularAdmisionEstado(Admision).GetAwaiter().GetResult();
        }
        public bool updateTipoCitaMG(string Tipo, int Admision, string Inicio)
        {
            return _agendaService.updateTipoCitaMG(Admision, Inicio, Tipo).GetAwaiter().GetResult();
        }
        public CXN_HORARIO DatosforMailSMS(int Admision)
        {
            return _agendaService.DatosforMailSMS(Admision).GetAwaiter().GetResult();
        }
        public bool ActualizarCelular(int admision, string celular)
        {
            return _pacientesService.ActualizarCelular(admision, celular).GetAwaiter().GetResult();
        }
        public bool Log(CXN_LOG_SENDER M)
        {
            return _logService.Log(M).GetAwaiter().GetResult();
        }
        public int CrearCitaMedica(CXN_HORARIO request)
        {
            return _agendaService.CrearCitaMedica(request).GetAwaiter().GetResult();
        }
        public otrosDatosPacienteHorario CargarAdmision(int admision, string filtro)
        {
            return _agendaService.CargarAdmision(admision, filtro).GetAwaiter().GetResult();
        }
        public int AnularAdmision(int admision, string usuario)
        {
            return _agendaService.AnularAdmision(admision, usuario).GetAwaiter().GetResult();
        }
        public int AnularRcCaja(int admision)
        {
            return _rcrCajaService.AnularRcCaja(admision).GetAwaiter().GetResult();
        }
        public bool getPendientes()
        {
            return _agendaService.getPendientes().GetAwaiter().GetResult();
        }
        public bool desbloquearEspacio(int admision, string usuario)
        {
            return _agendaService.desbloquearEspacio(admision, usuario).GetAwaiter().GetResult();
        }
        public List<RCCAJA> GetRcCaja(int admision)
        {
            return _rcrCajaService.GetRcCaja(admision).GetAwaiter().GetResult();
        }
        public Dictionary<int, string> getSaleConsultas(DateTime fecha, string tipo)
        {
            return _agendaService.getSaleConsultas(fecha, tipo).GetAwaiter().GetResult();
        }
        public bool UpdateSaleConsultas(int admision)
        {
            return _agendaService.UpdateSaleConsultas(admision).GetAwaiter().GetResult();
        }
        public CXN_HORARIO GenerarImprentaAutomatica(int Admision)
        {
            return _agendaService.GenerarImprentaAutomatica(Admision).GetAwaiter().GetResult();
        }
        public List<FirmasR> Firmas_Print(int admision, FirmasR F, bool autocomplete)
        {
            return _admisionesService.Firmas_Print(admision, F, autocomplete).GetAwaiter().GetResult();
        }
        public bool OpenAdmition(int admision) //HECHO
        {
            return _admisionesService.OpenAdmition(admision).GetAwaiter().GetResult();
        }
        public bool consumirAutorizacion(int paciente, string autorizacion, string estado)
        {
            return _agendaService.consumirAutorizacion(paciente, autorizacion, estado).GetAwaiter().GetResult();
        }
    }
}
