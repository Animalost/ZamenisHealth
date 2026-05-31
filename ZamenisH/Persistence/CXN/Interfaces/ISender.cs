using Domain.CXN;
using SMSSender;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ISender
    {
        bool Actualizar(CXN_EMAIL E);
        List<CXN_HORARIO> EnviarSMS(string TEnvio, DateTime Fecha);
        string Envia_SMS(string Celular_Class,
                               string Hor_Id_Class,
                               string T_Cita,
                               int Compañia,
                               string URL,
                               string UsuarioLogueado,
                               string URLFinal);

        List<CXN_HORARIO> EnviarEMAIL(string TEnvio, DateTime Fecha);
        (string TextoCuerpo, byte[] GoogleCalendar) Envia_Mail(string TCITA_CLASS,
                                                                       int Admision,
                                                                       string EmailPaciente,
                                                                       int Compañia,
                                                                       string Baja,
                                                                       string Email,
                                                                       DateTime FechaCita,
                                                                       DateTime HoraCita,
                                                                       string Paciente,
                                                                       string DireccionCia,
                                                                       string TelefonoCia,
                                                                       string Link);
    }
}
