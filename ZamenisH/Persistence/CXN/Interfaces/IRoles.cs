using Domain;

namespace Persistence.CXN.Interfaces
{
    public interface IRoles
    {
        CXN_DESKTOP_ROLES getDesktopRoles(string User);
        bool SaveRoles(CXN_DESKTOP_ROLES roles, string Usuario);
    }
}
