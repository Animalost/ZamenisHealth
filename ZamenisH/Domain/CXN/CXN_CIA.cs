using System;

namespace Domain.CXN
{
    public class CXN_CIA : OtrosDatosCIA
    {
        public string Com_Nombre { get; set; }
        public string Com_Identificacion { get; set; }
        public string Com_Direccion { get; set; }
        public string Com_Telefono { get; set; }
        public string Com_Resolucion { get; set; }
        public string Com_Resolucion_Electron { get; set; }
        public string Com_UsuarioGraba { get; set; }
        public int Com_Identificador { get; set; }
        public string Com_Cod_Prestador { get; set; }
        public string Com_Cod_Prestador_2 { get; set; }
        public string Com_Tipo_Doc { get; set; }
        public string Com_Email { get; set; }
        public string Com_Id { get; set; }
        public string Com_Logo { get; set; }
        public string Com_Nombre_SMS { get; set; }
        public string Com_Telefono_SMS { get; set; }
        public int Com_OP { get; set; }
        public int Com_Fac { get; set; }
        public int Com_Cotiza { get; set; }
        public int Com_DE { get; set; }
        public int Com_OM { get; set; }
        public int Com_PedPro { get; set; }
        public int Com_RIP { get; set; }
        public int Com_SMS { get; set; }
        public int Com_Doc_Electron { get; set; }
        public string Com_Prefijo_Electron { get; set; }
        public DateTime Com_Fecha_Electron { get; set; }
        public string Com_Numeracion_Electron { get; set; }
        public int Com_Doc_Electron_NC { get; set; }
        public string Com_Prefijo_Electron_NC { get; set; }
        public int Com_ConsContable { get; set; }
        public string Com_DVerifica { get; set; }

        public string Com_Resolucion_Soporte { get; set; }
        public int Com_Doc_Soporte { get; set; }
        public string Com_Prefijo_Soporte { get; set; }
        public string Com_Numeracion_Soporte { get; set; }
        public DateTime Com_Fecha_Soporte { get; set; }
        public string Com_Prefijo_Soporte_NC { get; set; }
        public int Com_Doc_Soporte_NC { get; set; }

        public string Diferenciador { get; set; }
        public int Com_Cierres { get; set; }
    }

    public class OtrosDatosCIA : ComplementoHorario
    {
        public byte[] Logo { get; set; }
        public DateTime FechaBase { get; set; }
        public string PacienteAseguradora { get; set; }
        public string PacienteDireccion { get; set; }
        public string PacienteIdentificacion { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteTelefono { get; set; }

        public byte[] FirmaMed { get; set; }
        public byte[] FirmaPac { get; set; }

        public string IdPaciente { get; set; }
        public string IdProfesional { get; set; }
        public string DirPaciente { get; set; }
        public string TelPaciente { get; set; }
    }

    public class imageSystem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
    }
}
