using System;

namespace Domain.CXN
{
    public class CXN_ENCUESTASATIS : complementSatis
    {
        public int Id { get; set; }
        public int Admision { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }
        public int P4 { get; set; }
        public int P5 { get; set; }
        public int P6 { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }
        public string Lugar { get; set; }
    }

  

    public class complementSatis : InformeSatisfaccion
    {
        public string Paciente { get; set; }
        public string Documento { get; set; }
        public byte[] Logo { get; set; }
        public DateTime Fecha { get; set; }
        public string P1t { get; set; }
        public string P2t { get; set; }
        public string P3t { get; set; }
        public string P4t { get; set; }
        public string P5t { get; set; }
        public string P6t { get; set; }
    }

    public class InformeSatisfaccion
    {
        public string Deficiente { get; set; }
        public string Malo { get; set; }
        public string Regular { get; set; }
        public string Bueno { get; set; }
        public string Excelente { get; set; }

        //PREGUNTA 1
        public string P1R1Percentual { get; set; }
        public string P1R1Cantidad { get; set; }
        public string P1R2Percentual { get; set; }
        public string P1R2Cantidad { get; set; }
        public string P1R3Percentual { get; set; }
        public string P1R3Cantidad { get; set; }
        public string P1R4Percentual { get; set; }
        public string P1R4Cantidad { get; set; }
        public string P1R5Percentual { get; set; }
        public string P1R5Cantidad { get; set; }
        public string P1R6Percentual { get; set; }
        public string P1R6Cantidad { get; set; }
        //PREGUNTA 2
        public string P2R1Percentual { get; set; }
        public string P2R1Cantidad { get; set; }
        public string P2R2Percentual { get; set; }
        public string P2R2Cantidad { get; set; }
        public string P2R3Percentual { get; set; }
        public string P2R3Cantidad { get; set; }
        public string P2R4Percentual { get; set; }
        public string P2R4Cantidad { get; set; }
        public string P2R5Percentual { get; set; }
        public string P2R5Cantidad { get; set; }
        public string P2R6Percentual { get; set; }
        public string P2R6Cantidad { get; set; }
        //PREGUNTA 3
        public string P3R1Percentual { get; set; }
        public string P3R1Cantidad { get; set; }
        public string P3R2Percentual { get; set; }
        public string P3R2Cantidad { get; set; }
        public string P3R3Percentual { get; set; }
        public string P3R3Cantidad { get; set; }
        public string P3R4Percentual { get; set; }
        public string P3R4Cantidad { get; set; }
        public string P3R5Percentual { get; set; }
        public string P3R5Cantidad { get; set; }
        public string P3R6Percentual { get; set; }
        public string P3R6Cantidad { get; set; }
        //PREGUNTA 4
        public string P4R1Percentual { get; set; }
        public string P4R1Cantidad { get; set; }
        public string P4R2Percentual { get; set; }
        public string P4R2Cantidad { get; set; }
        public string P4R3Percentual { get; set; }
        public string P4R3Cantidad { get; set; }
        public string P4R4Percentual { get; set; }
        public string P4R4Cantidad { get; set; }
        public string P4R5Percentual { get; set; }
        public string P4R5Cantidad { get; set; }
        public string P4R6Percentual { get; set; }
        public string P4R6Cantidad { get; set; }
        //PREGUNTA 5
        public string P5R1Percentual { get; set; }
        public string P5R1Cantidad { get; set; }
        public string P5R2Percentual { get; set; }
        public string P5R2Cantidad { get; set; }
        public string P5R3Percentual { get; set; }
        public string P5R3Cantidad { get; set; }
        public string P5R4Percentual { get; set; }
        public string P5R4Cantidad { get; set; }
        public string P5R5Percentual { get; set; }
        public string P5R5Cantidad { get; set; }
        public string P5R6Percentual { get; set; }
        public string P5R6Cantidad { get; set; }
        //PREGUNTA 6
        public string P6R1Percentual { get; set; }
        public string P6R1Cantidad { get; set; }
        public string P6R2Percentual { get; set; }
        public string P6R2Cantidad { get; set; }
        public string P6R3Percentual { get; set; }
        public string P6R3Cantidad { get; set; }
        public string P6R4Percentual { get; set; }
        public string P6R4Cantidad { get; set; }
        public string P6R5Percentual { get; set; }
        public string P6R5Cantidad { get; set; }
        public string P6R6Percentual { get; set; }
        public string P6R6Cantidad { get; set; }
        public string Periodo { get; set; }
        public int numEncuesta { get; set; }
        public int TotalEncuestas { get; set; }
    }
}
