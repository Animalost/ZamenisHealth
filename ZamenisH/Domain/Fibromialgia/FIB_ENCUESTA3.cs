using System;

namespace Domain.Fibromialgia
{
    public class FIB_ENCUESTA3 : FIB_A_TEXTOS
    {
        public int Id { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaEncuesta { get; set; }
        public string UsuarioRegistra { get; set; }
        public string Estado { get; set; }
        public string UsuarioCambia { get; set; }
        public DateTime FechaCambio { get; set; }
        public string Pregunta1 { get; set; }
        public string Pregunta2 { get; set; }
        public string Pregunta3 { get; set; }
        public string Pregunta4 { get; set; }
        public string Pregunta5 { get; set; }
        public string Pregunta6 { get; set; }
        public string Pregunta7 { get; set; }
        public string Pregunta8 { get; set; }
        public string Pregunta9 { get; set; }
        public string Pregunta10 { get; set; }
        public DateTime fechaEncuesta { get; set; }

    }
}
