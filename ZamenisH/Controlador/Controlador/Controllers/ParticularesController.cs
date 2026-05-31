using Controlador.Services;
using Domain;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers
{
    public  class ParticularesController
    {
        private CompañiaService CompañiaService;
        private ConveniosService ConveniosService;
        private ProductosService ProductosService;
        private ParticularesService ParticularesService;
        private FacturacionService facturacionService;
        private ReportesService reportesService;
        private PacientesService pacientesService;
        private CargosService cargosService;
        private AgendarCitaService agendarCitaService;

        public ParticularesController()
        {
            CompañiaService = new CompañiaService();
            ConveniosService = new ConveniosService();
            ProductosService = new ProductosService();
            ParticularesService = new ParticularesService();
            facturacionService = new FacturacionService();
            reportesService = new ReportesService();
            pacientesService = new PacientesService();
            cargosService = new CargosService();
            agendarCitaService = new AgendarCitaService();
        }

        public List<string> ObtenerListaPrestadores()
        {
            return CompañiaService.ObtenerListaPrestadores().GetAwaiter().GetResult();
        }

        public List<string> CargarServiciosxASE(int ase)
        {
            return ConveniosService.CargarServiciosxASE(ase).GetAwaiter().GetResult();
        }

        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return CompañiaService.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();  
        }

        public CXN_CONVENIOS ServicioCUP(int Ase, string Service)
        {
            return ConveniosService.ServicioCUP(Ase, Service).GetAwaiter().GetResult(); 
        }

        public (int valor, string item, string detalle) ConsultarValor(int ase, string code)
        {
            CXN_INVENTARIO invTemp = ProductosService.ConsultarValor(code, ase).GetAwaiter().GetResult();
            if (invTemp != null) 
            {
                (int valor, string item, string detalle) tuple = (invTemp.InvPrecio, invTemp.InvItem, invTemp.InvDetalle);
                return tuple;
            }

            return (0,"","");
        }

        public CXN_CIA getPrestadorbyCode(int code)
        {
            return CompañiaService.getPrestadorbyCode(code).GetAwaiter().GetResult();
        }

        public string insertService(CXN_CARGOS C)
        {
            return ParticularesService.insertService(C).GetAwaiter().GetResult();
        }

        public string insertarDocumento(CXN_FACTURA C)
        {
            return facturacionService.insertarDocumento(C).GetAwaiter().GetResult();    
        }

        public bool ConsecutivoActualiza(int code, int nuecons, string tdoc)
        {
            return CompañiaService.ConsecutivoActualiza(code, nuecons, tdoc).GetAwaiter().GetResult();
        }

        public List<CotizacionR> GenerarDocumento(int doc, int cia)
        {
            return reportesService.GenerarDocumento(doc, cia).GetAwaiter().GetResult(); 
        }

        public CXN_PACIENTES LlamarPacienteDOC(string tipoid, string numid)
        {
            return pacientesService.LlamarPacienteDOC(tipoid, numid).GetAwaiter().GetResult();
        }

        public ListaCarga Carga_Plantilla()
        {
            return cargosService.Carga_Plantilla().GetAwaiter().GetResult();
        }

        public List<string> ListaDocs()
        {
            return agendarCitaService.ListaDocs().GetAwaiter().GetResult();
        }
    }
}
