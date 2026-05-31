using System;

namespace Domain.CXN
{
    public class CXN_TOKENS_FHIR
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Fecha { get; set; }
        public int Prestador { get; set; }
    }
}
