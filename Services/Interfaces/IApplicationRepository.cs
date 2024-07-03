using angPOC.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace angPOC.Services
{
    /// <summary>
    /// Application endpoints  
    /// </summary>
    public  interface IApplicationRepository
    {
 
        Task<IEnumerable<Application>> GetApplicationsAsync(bool bActive = true);


        Task<Application> GetApplicationAsync(int id);

        bool ApplicationExists(int id);


        Task DeleteApplicationAsync(int id);


        Task SaveApplicationAsync(Application repo);



    }
}
