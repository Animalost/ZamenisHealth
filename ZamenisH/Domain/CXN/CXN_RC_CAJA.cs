using System;

namespace Domain.CXN
{
    public class CXN_RC_CAJA : CXN_CIA
    {
        public int Rc_Caja_Pac { get; set; }
        public int Rc_Caja_Ase { get; set; }
        public int Rc_Caja_Cia { get; set; }
        public DateTime Rc_Caja_Fecha { get; set; }
        public string Rc_Caja_UsrGraba { get; set; }
        public int Rc_Caja_Valor { get; set; }
        public int Rc_Caja_Adm { get; set; }
        public string Rc_Caja_Observacion { get; set; }
        public int Rc_Id { get; set; }

        public string Hor_DocFEModerador { get; set; }
        public string FormaPago { get; set; }
        public string Hor_DocFEModeradorCUFE { get; set; }
        public string Hor_ConceptoRecaudo { get; set; }
        public string Hor_DocFEModeradorRes { get; set; }
        public DateTime Hor_DocFEModeradorFechaHora { get; set; }
        public string Hor_DocFEModeradorNumeracion { get; set; }
        public string Hor_Cruce { get; set; }
        public DateTime Hor_FechaCruce { get; set; }
        public int Num_Cruce { get; set; }
    }

    public class ComplementoRCCaja
    {
        //otros datos
        public string PacienteIdentificacion { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteTelefono { get; set; }
        public string PacienteDireccion { get; set; }
        public string PacienteAseguradora { get; set; }
    }

    public class RCCAJA : Modelo
    {
        public int RC_ID { get; set; }
        public int Valor { get; set; }
        public int Cantidad { get; set; }
        public int Recibo { get; set; }
        public string Observacion { get; set; }
        public string Vendedor { get; set; }
        public string Letras { get; set; }
        public byte[] QRImage { get; set; }
        public string Correo { get; set; }
        public string Resolucion { get; set; }

        public string TDocReceptor { get; set; }
        public string DocReceptor { get; set; }
        public int AseIdentificator { get; set; }
        public string ResElectron { get; set; }
        public string PrefijoElectron { get; set; }
        public int NumeroElectron { get; set; }
        public DateTime FechaRealElectron { get; set; }
    }
}
