using System;

namespace Domain.INV
{
    public class INV_SALIDASSUB
    {
        public int Id { get; set; }
        public int CodPpal { get; set; }
        public int Cantidad { get; set; }
        public int Bodega { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
    }
}
