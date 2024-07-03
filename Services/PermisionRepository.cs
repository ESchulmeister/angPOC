using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using angPOC.Data;

namespace angPOC.Services
{
    public class PermisionRepository : IPermissionRepository

    {
        #region Variables

        private readonly UserDBContext _context;

        #endregion

        #region Constructors
        public PermisionRepository(UserDBContext context) => _context = context;




        #endregion

        /// <summary>
        /// Get List<>  application permissions
        /// </summary>
        /// <param name="appID">application id - master key</param>
        public async Task<IEnumerable<AppPermission>> GetApplicationPermissions(int appID)
        {
            IQueryable<AppPermission> query = _context.AppPermissions.Where(p => p.AppId == appID);

            return await query.ToListAsync();

        } 
        
   

        /// <summary>
        /// Save application permission
        /// </summary>
        /// <param name="appID">application id</param>
        /// <param name="repoPermission">permission entity</param>
        public async Task SavePermissionAsync(int appID, AppPermission repoPermission)
        {

            try
            {

                var app_ID = new SqlParameter("@appID", appID);

                var apID = new SqlParameter("@apID", repoPermission.ApId == 0 ? System.DBNull.Value : repoPermission.ApId);    //new vs existing permission

                var permName = new SqlParameter("@permName", repoPermission.PermName);

                var outID = new SqlParameter();
                outID.ParameterName = "@outID";
                outID.SqlDbType = SqlDbType.Int;
                outID.Direction = ParameterDirection.Output;

                await _context.Database.ExecuteSqlRawAsync("EXEC usp_execApplication @appID = {0} , " +
                    "@apID = {1}, " +
                    "@permName= {2}, " +
                    "@outID ={3}  OUT ", 
                    appID, apID, permName, outID);

                repoPermission.ApId = (int)outID.Value;
            }
            catch (SqlException oSqlException)   //stored procedure ex
            {
                if (oSqlException.Number == Constants.ErrorCode.InvaliPermission)
                {
                    throw new DataValidationException(oSqlException.Message);
                }

                throw;     // any other exeption
            } 
        }
    }
}
