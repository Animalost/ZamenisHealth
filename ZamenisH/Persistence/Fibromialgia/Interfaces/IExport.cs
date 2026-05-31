using System.Collections.Generic;
using Domain.Fibromialgia;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IExport
    {
        List<FIB_ENCUESTA1> ExportarEncuesta1(int Id);
        List<FIB_ENCUESTA2> ExportarEncuesta2(int Id);
        List<FIB_ENCUESTA3> ExportarEncuesta3(int Id);
    }
}
