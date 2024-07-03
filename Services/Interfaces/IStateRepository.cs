using System.Collections.Generic;
using System.Threading.Tasks;
using angPOC.Data;


namespace angPOC.Services
{
    /// <summary>
    /// State Endpoints 
    /// </summary>
    public interface IStateRepository
    {

        Task<IEnumerable<State>> GetStates();


    }
}
