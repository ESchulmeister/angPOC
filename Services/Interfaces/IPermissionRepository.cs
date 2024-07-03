using angPOC.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace angPOC.Services
{
    public interface IPermissionRepository
    {

        Task<IEnumerable<AppPermission>> GetApplicationPermissions(int appID);

        Task SavePermissionAsync(int appID, AppPermission  repo);

    }
}
