using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class CrearPacienteFast
    {
        private AgendarCitaService AgendarCitaService;
        private AseguradoraService AseguradoraService;
        private PacientesService PacientesService;

        public CrearPacienteFast()
        {
            AgendarCitaService = new AgendarCitaService();
            AseguradoraService = new AseguradoraService();
            PacientesService = new PacientesService();
        }

        public List<string> ListaRegimen()
        {
            return AgendarCitaService.ListaRegimen().GetAwaiter().GetResult();
        }

        public CXN_ASEGURADORA GetInfoFromAsebyName(string nameAse)
        {
            return AseguradoraService.GetInfoFromAsebyName(nameAse).GetAwaiter().GetResult();
        }

        public bool CrearClientes(CXN_PACIENTES P)
        {
            return PacientesService.CrearClientes(P).GetAwaiter().GetResult();
        }

        public List<CXN_ASEGURADORA> GetAseguradoras()
        {
            return AseguradoraService.GetAseguradoras().GetAwaiter().GetResult();
        }

        public List<string> ListaDocs()
        {
            return AgendarCitaService.ListaDocs().GetAwaiter().GetResult();
        }
    }
}
