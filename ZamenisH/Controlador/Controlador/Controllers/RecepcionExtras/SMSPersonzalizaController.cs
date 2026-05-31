using Controlador.Services;
using Domain.CXN;

namespace Controlador.Controllers.RecepcionExtras
{
    public class SMSPersonzalizaController
    {
        private readonly LogService logService;

        public SMSPersonzalizaController()
        {
            logService = new LogService();
        }

        public bool Log(CXN_LOG_SENDER M)
        {
            return logService.Log(M).GetAwaiter().GetResult();
        }
    }
}
