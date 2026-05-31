using Controlador.Services;
using Domain.CXN;
using System;
using System.Collections.Generic;
using System.IO;

namespace Controlador.Controllers.RecepcionExtras
{
    public class OrdenesPendientesController
    {
        private OMService _service;
        private PlanosService _planosService;

        public OrdenesPendientesController() 
        { 
            _service = new OMService();
            _planosService = new PlanosService();
        }

        public List<CXN_HORARIO> getListPendientes()
        {
            return _service.getListPendientes().GetAwaiter().GetResult();
        }

        public string ExpPlanoOrdenesPendientes(DateTime desde, DateTime hasta)
        {
            return _planosService.ExpPlanoOrdenesPendientes(desde, hasta).GetAwaiter().GetResult(); 
        }
    }
}
