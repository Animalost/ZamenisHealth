using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IRoles
    {
        CXN_ROLES getRoles(string User);
        bool updateRoles(CXN_ROLES R);
        bool insertRoles(CXN_ROLES R);
    }
}
