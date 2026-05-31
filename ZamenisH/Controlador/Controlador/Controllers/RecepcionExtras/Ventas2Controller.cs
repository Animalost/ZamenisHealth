using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class Ventas2Controller
    {
        private readonly ProductosService _productosService;

        public Ventas2Controller()
        {
            _productosService = new ProductosService();
        }

        public List<CXN_INVENTARIO> getProductbyName(string name)
        {
            return _productosService.getProductbyName(name).GetAwaiter().GetResult();
        }
    }
}
