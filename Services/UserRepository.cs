#undef TOKEN_SHORT_EXPIRATION

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using angPOC.Data;
using angPOC.Models;
using angPOC.Utilities;

namespace angPOC.Services
{

    public class UserRepository : IUserRepository
    {
        #region Variables

        private readonly UserDBContext _context;
        private readonly AppSettings _appSettings;

        #endregion

        #region Constructors
        public UserRepository(UserDBContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>JSON:
        /// {
        ///  "id": 0,
        ///  "login": "string",
        ///  "lastName": "string",
        ///  "firstName": "string",
        ///  "clock": "string",
        ///  "email": "user@example.com",
        ///  "createDate": "2021-09-30T17:58:20.688Z",
        ///  "updateDate": "2021-09-30T17:58:20.688Z",
        ///  "isActive": true,
        ///  "modifiedBy": "string"
        ///  }
        /// </returns>
        public async Task<User> GetUserAsync(int id)
        {
            IQueryable <User> query =_context.Users.Where(c => c.UsrId == id);

            return await query.FirstOrDefaultAsync();
        
        }


        /// <summary>
        /// Get List of active  users
        /// </summary>
        /// <returns>JSON:
        /// [
        /// {
        ///  "id": 0,
        /// "login": "string",
        ///  "lastName": "string",
        ///  "firstName": "string",
        ///  "clock": "string",
        ///  "email": "user@example.com",
        ///  "createDate": "2021-09-30T17:58:20.688Z",
        ///  "updateDate": "2021-09-30T17:58:20.688Z",
        ///  "isActive": true,
        ///  "modifiedBy": "string"
        ///  }
        /// ]
        /// </returns>
        public async Task<IEnumerable<User>> GetUsersAsync()
        {

            IQueryable<User> query = _context.Users.Where(c => c.UsrActive == true);
            return await query.ToListAsync();
        }


        /// <summary>
        /// Get List of users by application ID
        /// </summary>
        /// <param name="appID">application id</param>
        /// <returns>JSON Array:
        ///[
        ///{
        ///  "id": 0,
        /// "login": "string",
        ///  "lastName": "string",
        ///  "firstName": "string",
        ///  "clock": "string",
        ///  "email": "user@example.com",
        ///  "createDate": "2021-09-30T17:58:20.688Z",
        ///  "updateDate": "2021-09-30T17:58:20.688Z",
        ///  "isActive": true,
        ///  "modifiedBy": "string"
        ///  }
        ///  [, ... ]
        ///  ]
        /// </returns>
        public async Task<IEnumerable<User>> GetUsersByAppAsync(int appID)
        {
            try
            {
                var id = new SqlParameter { ParameterName = "@appID", Value = appID };

                IQueryable<User> query = _context.Users
                                .FromSqlRaw("EXECUTE usp_selUsersByApplication @appID", id);

                return await query.ToListAsync();
            }
            catch (SqlException oSqlException)   //stored procedure ex
            {
                if (oSqlException.Number == Constants.ErrorCode.InvalidApplication)
                {
                    throw new DataValidationException(oSqlException.Message);
                }

                throw;
            }

        }


        /// <summary>
        /// Dupe  check
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>boolean flag</returns>
        public bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UsrId == id);
        }


        /// <summary>
        /// Update  user data
        /// </summary>
        /// <param name="user">
        /// JSON:
        ///  {
        ///  "id": 0,
        /// "login": "string",
        ///  "lastName": "string",
        ///  "firstName": "string",
        ///  "clock": "string",
        ///  "email": "user@example.com",
        ///  "createDate": "2021-09-30T17:58:20.688Z",
        ///  "updateDate": "2021-09-30T17:58:20.688Z",
        ///  "isActive": true,
        ///  "modifiedBy": "string"
        ///  }
        /// </param>
        /// <returns></returns>
        public async Task UpdateUserAsync(User user)
        {
            var oUser = _context.Entry(user);

            oUser.State = EntityState.Modified;
            oUser.Property(x => x.UsrCreatedDate).IsModified = false;

            user.UsrModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

        }



        public async Task<IEnumerable<User>> GetAll()
        {

            IQueryable<User> query = _context.Users;

            return await query.ToListAsync();
        }

        private string GenerateJwtToken(User user)
        {
            // generate token  - valid for 2 days
            var tokenHandler = new JwtSecurityTokenHandler();
#pragma warning disable CS8604 // Possible null reference argument.
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
#pragma warning restore CS8604 // Possible null reference argument.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim(Constants.Claims.ID, user.UsrId.ToString()),
                    new Claim(Constants.Claims.ApplicationID, Constants.Claims.ApplicationValue)
                }),

#if TOKEN_SHORT_EXPIRATION
                Expires = DateTime.UtcNow.AddSeconds(10),
#else
                Expires = DateTime.UtcNow.AddDays(2),
#endif
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            //create & write-out newly generated token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        /// <summary>
        /// Save User data
        /// </summary>
        /// <param name="repoUser"></param>
        /// <returns></returns>
        public async Task SaveUserAsync(User repoUser)
        {
            try
            {

                var usrID = new SqlParameter("@usrID", (repoUser.UsrId == 0) ? System.DBNull.Value : repoUser.UsrId);
                var usrLogin = new SqlParameter("@usrLogin", repoUser.UsrLogin);
                var usrLastName = new SqlParameter("@usrLastName", repoUser.UsrLastName);
                var usrFirstName = new SqlParameter("@usrFirstName", repoUser.UsrFirstName);
                var usrClock = new SqlParameter("@usrClock", (String.IsNullOrWhiteSpace(repoUser.UsrClock)) ? System.DBNull.Value : repoUser.UsrClock);
                var usrStateID = new SqlParameter("@usrStateID", repoUser.UsrStateId);
                var usrEmail = new SqlParameter("@usrEmail", repoUser.UsrEmail);

                var outID = new SqlParameter();
                outID.ParameterName = "@outID";
                outID.SqlDbType = SqlDbType.Int;
                outID.Direction = ParameterDirection.Output;

                await _context.Database.ExecuteSqlRawAsync("EXEC usp_execUser @usrID = {0}," +
                    " @usrLogin = {1}," +
                    " @usrLastName = {2}," +
                    " @usrFirstName = {3}," +
                    " @usrClock = {4}," +
                    " @usrEmail ={5}, " +
                    " @usrStateID = {6}, " +
                    " @outID = {7} OUT ",
                    usrID, usrLogin, usrLastName, usrFirstName, usrClock, usrEmail,  usrStateID, outID);

                //return user ID - output parameter
                repoUser.UsrId = (int)outID.Value;

            }
            catch (SqlException oSqlException)   //stored procedure ex
            {
                if (oSqlException.Number == Constants.ErrorCode.InvalidUser)
                {
                    throw new DataValidationException(oSqlException.Message);
                }

                throw;  // any other exception
            }


        }
#endregion
    }
}
