using angPOC.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace angPOC.Services
{
    public class StateRepository : IStateRepository

    {
        #region Variables

        private readonly UserDBContext _context;

        #endregion

        #region Constructors
        public StateRepository(UserDBContext context) => _context = context;


        #endregion



        public async Task<IEnumerable<State>> GetStates()
        {
            IQueryable<State> query = _context.States;  

            return await query.ToListAsync();

        }
    }
}
