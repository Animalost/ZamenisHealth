using Controlador.Services;

using Domain.CXN;

using System;
using System.Collections.Generic;

namespace Controlador.Controllers.Contenedor
{
    public class SMSDisponiblesController
    {
        private readonly CompañiaService compa;

        public SMSDisponiblesController() 
        {
            compa = new CompañiaService();
        }

        public List<string> ObtenerListaPrestadores()
        {
            return compa.ObtenerListaPrestadores().GetAwaiter().GetResult();
        }

        public CXN_CIA getPrestadorbyName(string NamePrestador)
        {
            return compa.getPrestadorbyName(NamePrestador).GetAwaiter().GetResult();
        }
    }
}
