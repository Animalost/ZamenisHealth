using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IEncuestas
    {
        List<FIB_ENCUESTA1> getPreviosXPacEncuesta1(string tdoc, string doc);
        List<FIB_ENCUESTA1> getPreviosGeneralEncuesta1();
        List<FIB_ENCUESTA2> getPreviosXPacEncuesta2(string tdoc, string doc);
        List<FIB_ENCUESTA2> getPreviosGeneralEncuesta2();
        List<FIB_ENCUESTA3> getPreviosXPacEncuesta3(string tdoc, string doc);
        List<FIB_ENCUESTA3> getPreviosGeneralEncuesta3();
        bool ExcluirEncuesta(int Posision, string TipoEncuesta, string User);
    }
}
