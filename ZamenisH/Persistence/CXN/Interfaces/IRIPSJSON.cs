using Domain;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IRIPSJSON
    {
        Transaccion GenrateIndividual(RIPS_Class R);
        Dictionary<string, Transaccion> GenrateTotal(RIPS_Class R);
        TransaccionDocker GenrateTotalMinSalud(int ciaRIPS, string fac, string xmlB64);
    }
}
