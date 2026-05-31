using System;

namespace Domain.CXN_ADJUNTOS
{
    public class Adj_Archivos
    {
        public int Adj_Id { get; set; }
        public string Adj_Tipo { get; set; }
        public string Adj_Usr_Graba { get; set; }
        public DateTime Adj_Fecha { get; set; }
        public byte[] Adj_Archivo { get; set; }
        public int Adj_Paciente { get; set; }
        public string Adj_Observacion { get; set; }
        public string Adj_Clase { get; set; }
    }
}
