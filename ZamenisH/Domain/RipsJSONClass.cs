using System.Collections.Generic;

namespace Domain
{
    public class TransaccionDocker
    {
        public RipsDocker rips { get; set; }
        public string xmlFevFile { get; set; }  // Se agrega el campo

        public TransaccionDocker()
        {
            rips = new RipsDocker();
        }
    }

    public class RipsDocker
    {
        public string numDocumentoIdObligado { get; set; }
        public string numFactura { get; set; }
        public string tipoNota { get; set; }
        public string numNota { get; set; }
        public List<Usuario> usuarios { get; set; }

        public RipsDocker()
        {
            usuarios = new List<Usuario>();
        }
    }


    public class Transaccion
    {
        public string numDocumentoIdObligado { get; set; }
        public string numFactura { get; set; }
        public string tipoNota { get; set; }
        public string numNota { get; set; }
        public List<Usuario> usuarios { get; set; }

        public Transaccion()
        {
            usuarios = new List<Usuario>();
        }
    }
    public class Usuario
    {
        public string codMunicipioResidencia { get; set; }
        public string codPaisOrigen { get; set; }
        public string codPaisResidencia { get; set; }
        public string codSexo { get; set; }
        public string codZonaTerritorialResidencia { get; set; }
        public int consecutivo { get; set; }
        public string fechaNacimiento { get; set; }
        public string incapacidad { get; set; }
        public string numDocumentoIdentificacion { get; set; }
        public string tipoDocumentoIdentificacion { get; set; }
        public string tipoUsuario { get; set; }
        public Servicios servicios { get; set; }


        public Usuario()
        {
            servicios = new Servicios();
        }
    }
    public class Servicios
    {
        public List<Consultas> consultas { get; set; }
        public List<Procedimientos> procedimientos { get; set; }
        public List<OtrosServicios> otrosServicios { get; set; }
    }
    public class Consultas
    {
        public string codPrestador { get; set; }
        public string fechaInicioAtencion { get; set; }
        public string numAutorizacion { get; set; }
        public string codConsulta { get; set; }
        public string modalidadGrupoServicioTecSal { get; set; }
        public string grupoServicios { get; set; }
        public int codServicio { get; set; }
        public string finalidadTecnologiaSalud { get; set; }
        public string causaMotivoAtencion { get; set; }
        public string codDiagnosticoPrincipal { get; set; }
        public string codDiagnosticoRelacionado1 { get; set; }
        public string codDiagnosticoRelacionado2 { get; set; }
        public string codDiagnosticoRelacionado3 { get; set; }
        public string tipoDiagnosticoPrincipal { get; set; }
        public string tipoDocumentoIdentificacion { get; set; }
        public string numDocumentoIdentificacion { get; set; }
        public int vrServicio { get; set; }
        public string conceptoRecaudo { get; set; }
        public int valorPagoModerador { get; set; }
        public string numFEVPagoModerador { get; set; }
        public int consecutivo { get; set; }
    }
    public class Procedimientos
    {
        public string codPrestador { get; set; }
        public string fechaInicioAtencion { get; set; }
        public string idMIPRES { get; set; }
        public string numAutorizacion { get; set; }
        public string codProcedimiento { get; set; }
        public string viaIngresoServicioSalud { get; set; }
        public string modalidadGrupoServicioTecSal { get; set; }
        public string grupoServicios { get; set; }
        public int codServicio { get; set; }
        public string finalidadTecnologiaSalud { get; set; }
        public string tipoDocumentoIdentificacion { get; set; }
        public string numDocumentoIdentificacion { get; set; }
        public string codDiagnosticoPrincipal { get; set; }
        public string codDiagnosticoRelacionado { get; set; }
        public string codComplicacion { get; set; }
        public int vrServicio { get; set; }
        public string conceptoRecaudo { get; set; }
        public int valorPagoModerador { get; set; }
        public string numFEVPagoModerador { get; set; }
        public int consecutivo { get; set; }
    }
    public class OtrosServicios
    {
        public string codPrestador { get; set; }
        public string numAutorizacion { get; set; }
        public string idMIPRES { get; set; }
        public string fechaSuministroTecnologia { get; set; }
        public string tipoOS { get; set; }
        public string codTecnologiaSalud { get; set; }
        public string nomTecnologiaSalud { get; set; }
        public int cantidadOS { get; set; }
        public string tipoDocumentoIdentificacion { get; set; }
        public string numDocumentoIdentificacion { get; set; }
        public int vrUnitOS { get; set; }
        public int vrServicio { get; set; }
        public string conceptoRecaudo { get; set; }
        public int valorPagoModerador { get; set; }
        public string numFEVPagoModerador { get; set; }
        public int consecutivo { get; set; }
    }
}
