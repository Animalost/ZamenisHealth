using System;

namespace Domain.CXN
{
    public class CXN_TOKENS
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Hora { get; set; }
        public int CodPrestador { get; set; }
    }
}
