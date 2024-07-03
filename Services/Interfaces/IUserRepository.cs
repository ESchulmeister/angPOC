using System.Collections.Generic;
using System.Threading.Tasks;
using angPOC.Data;
using angPOC.Models;


namespace angPOC.Services
{
    /// <summary>
    /// User Endpoints 
    /// </summary>
    public interface IUserRepository
    {

        bool UserExists(int id);

        Task<IEnumerable<User>> GetUsersAsync();

        Task<IEnumerable<User>> GetAll();

        Task<User> GetUserAsync(int id);

        Task<IEnumerable<User>> GetUsersByAppAsync(int appID);

        Task UpdateUserAsync(User oUser);

        Task SaveUserAsync(User repo);


    }
}
