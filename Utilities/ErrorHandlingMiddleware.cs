using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using enc = System.Text.Encoding;
using log4net;

using angPOC.Services;

namespace angPOC.Utilities
{
    /// <summary>
    /// /Common api error handler
    /// Gets invoked on any application error  - ref. Startup.cs.  
    /// </summary>
    public class ErrorHandlingMiddleware
    {

        private readonly AppSettings? _appSettings ;
        private readonly ILog? _logger;

        private readonly RequestDelegate _next;
        //private HttpRequest? oRequest;
        private const string  General_Error = "An Unexpected  error has occurred. Please contact the system administrator.";



        #region constructor
        public ErrorHandlingMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
            _logger = LogManager.GetLogger("ErrorHandlingMiddleware");

        }
        #endregion


        #region Methods

        /// <summary>
        /// Request invocation
        /// </summary>
        /// <param name="context">incoming http context</param>
        /// <param name="repo"></param>
        /// <param name="_antiForgery"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IUserRepository repo, IAntiforgery _antiForgery)
        {
            try
            {
                // expecting : `Bearer ${user.token}`
                string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                this.ValidateAntiForgery(context, _antiForgery);

                if (token != null)   //User is logged in
                {
                    await this.AttachUserToContext(context, repo, token);
                }


                AntiforgeryTokenSet? tokens = _antiForgery.GetAndStoreTokens(context);

                string? _token = tokens?.RequestToken;

                if (tokens != null)
                {
                    context.Response.Cookies.Append(Constants.AntiForgery.Cookie, 
                                        value: _token, 
                                        options: new CookieOptions
                                        {
                                            HttpOnly = false,
                                            Secure = true
                                        });
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Validate AntiForgery token
        /// </summary>
        /// <param name="context">incoming http context</param>
        /// <param name="_antiForgery"></param>
        /// , IAntiforgery _antiForgery, HttpRequest oRequest
        private void ValidateAntiForgery(HttpContext context, IAntiforgery _antiForgery)
        {
            var oRequest = (context == null) ? null : context.Request;

            string sHeaderName = Constants.AntiForgery.Header.ToLower();       // header names are passed by Angular in lower case
            if (oRequest == null || !oRequest.Headers.ContainsKey(sHeaderName))
            {
                return;
            }

            string sHeaderValue = oRequest.Headers[sHeaderName];
            if (String.IsNullOrWhiteSpace(sHeaderValue))
            {
                return;
            }

            string? sCookieValue = oRequest.Cookies[Constants.AntiForgery.Cookie];
            if (String.Compare(sCookieValue, sHeaderValue, false) != 0)
            {
                throw new ForbiddenException("Anti-forgery failure");
            }
        }

        /// <summary>
        /// Handle Request Error
        /// </summary>
        /// <param name="oHttpContext"></param>
        /// <param name="oException">application error</param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext oHttpContext, Exception oException)
        {

             log4net.ILog log = log4net.LogManager.GetLogger(typeof(ErrorHandlingMiddleware));

            var sErrorMsg = string.Empty;

            if (oException.InnerException == null)    //Message specified @ request,unhandled exeption being thrown 
            {
                sErrorMsg = oException.ToString();
            }
            else
            {
                sErrorMsg = oException.InnerException.ToString();
            }


            log.Error(sErrorMsg);


            int iStatusCode = (int)HttpStatusCode.InternalServerError;
            string sMessage = oException.Message.Replace("\r\n", " ");

            sMessage = General_Error;

            //type of  exption thrown:
            switch (oException)
            {
                case SqlException     //database specific sql ex passed @ THROW, e.g. network  connection error
                AmbiguousMatchException:    
                    break;
                case DataValidationException 
                     BadRequestException:
                    iStatusCode = (int)HttpStatusCode.BadRequest;
                    sMessage = oException.Message;
                    break;
                case NotFoundException:
                    iStatusCode = (int)HttpStatusCode.NotFound;
                    sMessage = oException.Message;
                    break;
                case DbUpdateException:
                    break;
            }

            var oErrorMessageObject = new { Message = sMessage};
            var oResponse = oHttpContext.Response;

            oResponse.ContentType = "application/json";
            oResponse.StatusCode = iStatusCode;

            //write  out error message
            sMessage = JsonConvert.SerializeObject(oErrorMessageObject);
            await oResponse.WriteAsync(sMessage);
        }



        /// <summary>
        /// Upon successfull login, JWT 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="repo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task AttachUserToContext(HttpContext context, IUserRepository repo, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                string? _secret = _appSettings == null ? string.Empty : _appSettings.Secret;
                
                var aBytes = enc.ASCII.GetBytes(_secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(aBytes),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                if (jwtToken.HasExpired())
                {
                    context.Items[Constants.Keys.Context.User] = null;
                    return;
                }

                //match claim type - @ user ID
                var oClaim = jwtToken.Claims.FirstOrDefault(x => String.Compare(x.Type, Constants.Claims.ID, false) == 0);
                if(oClaim == null)
                {
                    return;
                }

                var userId = 0;
                if(!int.TryParse(oClaim.Value, out userId))
                {
                    return;
                }

                //User information
                var user = await repo.GetUserAsync( userId);

                // attach user to context on successful jwt validation
                context.Items[Constants.Keys.Context.User] = user; 
            }
            catch(SecurityTokenExpiredException)
            {
                context.Items[Constants.Keys.Context.User] = null;
                return;
            }
            catch(Exception) 
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
                //throw possible db error to display general error
                throw;

            }
        }

     
        #endregion 
    }
}
