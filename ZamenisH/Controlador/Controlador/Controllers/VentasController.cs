using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public class VentasController
    {
        private CompañiaService compañiaService;
        private VentasService ventasService;
        private PacientesService pacientesService;
        private ProductosService productosService;
        private AgendarCitaService agendarCitaService;

        public VentasController()
        {
            compañiaService = new CompañiaService();
            ventasService = new VentasService();
            pacientesService = new PacientesService();
            productosService = new ProductosService();
            agendarCitaService = new AgendarCitaService();
        }

        public CXN_CIA getPrestadorbyCode(int code)
        {
            return compañiaService.getPrestadorbyCode(code).GetAwaiter().GetResult();
        }

        public bool ConsecutivoActualiza(int code, int nuecons, string tdoc)
        {
            return compañiaService.ConsecutivoActualiza(code, nuecons, tdoc).GetAwaiter().GetResult();
        }

        public bool insertarVenta(CXN_VENTAS P)
        {
            return ventasService.insertarVenta(P).GetAwaiter().GetResult();
        }

        public CXN_PACIENTES LlamarPacienteOnlyDOC(string numid)
        {
            return pacientesService.LlamarPacienteOnlyDOC(numid).GetAwaiter().GetResult();
        }

        public List<string> ObtenerListaPrestadores()
        {
            return compañiaService.ObtenerListaPrestadores().GetAwaiter().GetResult();
        }

        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return compañiaService.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();
        }

        public List<CXN_VENTAS> getPrevios(int idpac)
        {
            return ventasService.getPrevios(idpac).GetAwaiter().GetResult();
        }

        public CXN_INVENTARIO getProductbyCode(string code)
        {
            return productosService.getProductbyCode(code).GetAwaiter().GetResult();
        }

        public List<string> ListaDocs()
        {
            return agendarCitaService.ListaDocs().GetAwaiter().GetResult();
        }
    }
}
