using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public  class PreciosController
    {
        private ReportesService reportesService;
        private ProductosService productosService;

        public PreciosController()
        {
            reportesService = new ReportesService();
            productosService = new ProductosService();
        }

        public string listaPrecios(int convenio)
        {
            return reportesService.listaPrecios(convenio).GetAwaiter().GetResult();
        }

        public List<CXN_INVENTARIO> getAllElements(string code, int ase)
        {
            return productosService.getAllElements(code, ase).GetAwaiter().GetResult();
        }

    }
}
