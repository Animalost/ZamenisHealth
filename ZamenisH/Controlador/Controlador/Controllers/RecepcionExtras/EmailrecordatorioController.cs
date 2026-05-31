using Controlador.Services;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Controllers.RecepcionExtras
{
    public class EmailrecordatorioController
    {
        private EmailService emailService;

        public EmailrecordatorioController()
        {
            emailService = new EmailService();
        }

        public List<CXN_EMAIL> getAllEmails()
        {
            return emailService.getAllEmails().GetAwaiter().GetResult();
        }

        public CXN_EMAIL Datos_Mail(string email)
        {
            return emailService.Datos_Mail(email).GetAwaiter().GetResult();
        }
    }
}
