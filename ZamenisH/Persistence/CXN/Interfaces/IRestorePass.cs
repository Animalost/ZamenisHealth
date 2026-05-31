using Domain.CXN;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.CXN.Interfaces
{
    public interface IRestorePass
    {
        int InsertRestorePass(CXN_RESTOREPASS R);
    }
}
