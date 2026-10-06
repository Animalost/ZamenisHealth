namespace Domain
{
    public class CXN_DESKTOP_ROLES
    {
        public int Id { get; set; }
        public string Recepcion { get; set; }
        public string Administracion { get; set; }
        public string Opciones { get; set; }
        public string Enfermeria { get; set; }
        public string MedicinaGeneral { get; set; }
        public string Gerencial { get; set; }
        public string Fisiatria { get; set; }
        public string Usuario { get; set; }
    }

    public class Recepcion
    {
        public string AgendaMedica { get; set; }
        public string Ventas { get; set; }
		public string ListarPrecios { get; set; }
        public string CrearEditarPaciente { get; set; }
        public string Cargos { get; set; }
        public string Cotizaciones { get; set; }
        public string VerAsistencia { get; set; }
        public string CopiaDocumentos { get; set; }
    }
    public class Administracion
    {
        public string Facturacion { get; set; }
        public string Anulaciones { get; set; }
        public string Formatos { get; set; }
        public string Cargos { get; set; }
        public string Inventarios { get; set; }
        public string Mensajero { get; set; }
        public string Rips { get; set; }
        public string Adherencia { get; set; }
        public string Envios { get; set; }
        public string BarCodes { get; set; }
        public string Autorizaciones { get; set; }
        public string RDA { get; set; }
        public AdministracionDetalles AdministracionDetalles { get; set; }
    }
    public class AdministracionDetalles
    {
        public string GenerarFactura { get; set; }
        public string Reportes { get; set; }
        public string DetalleGrupal { get; set; }
        public string FacturaAbierta { get; set; }
        public string Homologos { get; set; }
        public string FacturacionElectronica { get; set; }
        public string ReportesPagos { get; set; }
    }
    public class Opciones
    {
        public string Compañias { get; set; }
        public string CrearEditarUsuario { get; set; }
        public string Productos { get; set; }
        public string Horarios { get; set; }
        public string Convenios { get; set; }
        public string Preferencias { get; set; }
        public string Festivos { get; set; }
        public string CIE10 { get; set; }
        public string ECuentas { get; set; }
        public string Compras { get; set; }
        public string Bodegas { get; set; }
    }
    public class Enfermeria
    {
        public string Notas { get; set; }
        public string Plantillas { get; set; }
        public string Imagenes { get; set; }
        public string NAclaratoria { get; set; }
        public string SearchImages { get; set; }
        public string Inventario { get; set; }
        public string CManejo { get; set; }
        public string Registros { get; set; }
        public string Estadisticas { get; set; }
        public string Cargos { get; set; }
        public string SubirDocumentos { get; set; }
        public string DocumentosWEB { get; set; }
    }
    public class MedicinaGeneral
    {
        public string Historia { get; set; }
        public string Registros { get; set; }
        public string Retomar { get; set; }
        public string CManejo { get; set; }
        public string Subir { get; set; }
        public string Estadistica { get; set; }
        public string Nota { get; set; }
        public string Inventario { get; set; }
        public string DocumentosWEB { get; set; }
        public string GrabarImagenes { get; set; }
        public string Ordenes { get; set; }
        public string Solicitudes { get; set; }
        public string BuscarImagenes { get; set; }
        public string Consentimientos { get; set; }
        public string Salidas { get; set; }
    }
    public class Gerencial
    {
        public string FacturaPaciente { get; set; }
        public string Reportes { get; set; }
        public string Consentimientos { get; set; }
        public string EliminarCierreCaja { get; set; }
    }
    public class Fisiatria
    {
        public string CrearHistoria { get; set; }
        public string Retomar { get; set; }
        public string SubirHistoria { get; set; }
        public string NotaAclaratoria { get; set; }
        public string CompletarJuntas { get; set; }
        public string FirmarHistorias { get; set; }
        public string CrearOrdenes { get; set; }
        public string BuscarRegistros { get; set; }
    }
    
}
