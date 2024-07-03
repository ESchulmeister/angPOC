using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using angPOC.Data;

namespace angPOC.Services
{

    public class ApplicationRepository : IApplicationRepository
    {
        #region Variables

        private readonly UserDBContext _context;

        #endregion

        #region Constructors
        public ApplicationRepository(UserDBContext context) => _context = context;


        #endregion

        #region Methods    

        /// <summary>
        /// Get Application By ID
        /// </summary>
        /// <param name="appID">application id</param>
        public async Task<Application> GetApplicationAsync(int appID)
        {

            IQueryable<Application> query = _context.Applications.Where(c => c.AppId == appID); ;

            return await query.FirstOrDefaultAsync();
        }


        /// <summary>
        /// Get List of Applications
        /// </summary>
        /// <returns>
        /// <param  name="bActive">Default - active</param>
        public async Task<IEnumerable<Application>> GetApplicationsAsync(bool bActive = true)
        {
            IQueryable<Application> query = _context.Applications;

            return await query.ToListAsync();
         
        }

    
        /// <summary>
        /// Duplicate check By Id
        /// </summary>
        /// <param name="id">JSON "id" value</param>
        public bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.AppId == id);
        }

        /// <summary>
        /// Delete Application
        /// </summary>
        /// <param name="appID">application id - to be deleted</param>
        public async Task DeleteApplicationAsync(int id)
        {
            try
            {
                var appID = new SqlParameter { ParameterName = "@appID", Value = id };

                await _context.Database.ExecuteSqlRawAsync("usp_deleteApplication @appID", appID);
            }
            catch(SqlException oSqlException)   //stored procedure ex
            {
                if(oSqlException.Number == Constants.ErrorCode.InvalidApplication)
                {
                    throw new DataValidationException(oSqlException.Message);
                }

                throw;
            }           

        }

   
        /// <summary>
        /// Save application
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        public async Task SaveApplicationAsync(Application repo)
        {
            
            try
            {


                var appID = new SqlParameter("@appID", (repo.AppId == 0) ?  DBNull.Value : repo.AppId);
                var appName = new SqlParameter("@appName", repo.AppName);

                var outID = new SqlParameter();
                outID.ParameterName = "@outID";
                outID.SqlDbType = SqlDbType.Int;
                outID.Direction = ParameterDirection.Output;

                await _context.Database.ExecuteSqlRawAsync("EXEC usp_execApplication @appID = {0} , @appName = {1}, @outID ={2}  OUT ", appID, appName, outID);

                repo.AppId = (int)outID.Value;

            }
            catch (SqlException oSqlException)   //stored procedure ex
            {
                if (oSqlException.Number == Constants.ErrorCode.InvalidApplication)
                {
                    {
                        throw new DataValidationException(oSqlException.Message);
                    }

                    throw;
                }

                throw;
            }
        }

        #endregion
    }
}
