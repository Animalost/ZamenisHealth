using System;
using System.Collections.Generic;

namespace Domain.CXN
{
    public class CXN_PACIENTES : OtrosDatos
    {
        public string Pac_TipoId { get; set; }
        public string Pac_IdNum { get; set; }
        public string Pac_PrimerN { get; set; }
        public string Pac_SegundoN { get; set; }
        public string Pac_PrimerA { get; set; }
        public string Pac_SegundoA { get; set; }
        public DateTime Pac_FechaNto { get; set; }
        public string Pac_Sexo { get; set; }
        public string Pac_Telefono { get; set; }
        public string Pac_TelefonoAux { get; set; }
        public string Pac_Direccion { get; set; }
        public string Pac_Email { get; set; }
        public string Pac_Mun_Cod { get; set; }
        public string Pac_Zona { get; set; }
        public string Pac_Localidad { get; set; }
        public int Pac_Aseguradora { get; set; }
        public string Pac_Acudiente { get; set; }
        public string Pac_Parentesco { get; set; }
        public string Pac_DireccionAcu { get; set; }
        public string Pac_TelefonoAcu { get; set; }
        public string Pac_CorreoAcu { get; set; }
        public string Pac_Dep_Cod { get; set; }
        public string Pac_UsrGraba { get; set; }
        public string Pac_Regimen { get; set; }
        public string Pac_Usr_Web { get; set; }
        public string Pac_Pass_Web { get; set; }
        public int Pac_Id { get; set; }
        public string Pac_Doble { get; set; }
        public string Pac_2VXS { get; set; }
        public string Pac_FibInf { get; set; } 
        public string Pac_Especial { get; set; } 
        public string Pac_Contrato { get; set; }
        public string Pac_PaisOrigen { get; set; }
        public string Pac_Residencia { get; set; }
        public string Pac_Categoria { get; set; }
        public string Pac_ECivil { get; set; }
        public string Pac_Ocupacion { get; set; }
        public string VIH { get; set; }
        public string Hepatitis { get; set; }
        public string Discapacidad { get; set; }
        public string Etnia { get; set; }      
        public DateTime HoraNto { get; set; }
        public string IdentidadGenero { get; set; }
        public List<CXN_PACIENTES> ListaPacientes { get; set; }
    }

    public class OtrosDatos
    {
        public string AseguradoraNombre { get; set; }
    }
}
