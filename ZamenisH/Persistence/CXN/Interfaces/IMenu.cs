using System.Collections.Generic;
using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IMenu
    {
        List<string> getCausaExterna();
        List<CXN_MENUHGMG> getMenus();
        List<string> getPosPatologia();
        List<string> getPosPatologia2();
    }
}
