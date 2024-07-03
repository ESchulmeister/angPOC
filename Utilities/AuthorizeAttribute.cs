using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using angPOC.Services;
using angPOC.Utilities;


/// <summary>
/// Filter configuring request authorization
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public AuthorizeAttribute() : base()
    {
    }


    public string? ErrorMsg { get; set; }

    /// <summary>
    /// Get invoked w [Authorize]  controller attribute - POST/PUT/DELETE/PATCH actions
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? sHeaderToken = context.AuthorizationToken();

        bool bAccept = false;
        if (!String.IsNullOrWhiteSpace(sHeaderToken))
        {
            string sSessionToken = context.SessionToken();

            var tokenHandler = new JwtSecurityTokenHandler();
            var oJwtSecurityToken = tokenHandler.ReadJwtToken(sHeaderToken);

            if (String.IsNullOrWhiteSpace(sSessionToken))
            {
                bAccept = this.ValidateTokenClaims(sHeaderToken, context, oJwtSecurityToken);
            }
            else
            {
                bAccept = this.ValidateSessionToken(sHeaderToken, sSessionToken, oJwtSecurityToken);
            }
        }

        if (!bAccept)
        {
            // not logged in
            context.Result = new JsonResult(new { message = $"Unauthorized:{this.ErrorMsg}" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }

    /// <summary>
    /// Validate session token
    /// </summary>
    /// <param name="sHeaderToken"></param>
    /// <param name="sSessionToken"></param>
    /// <param name="oJwtSecurityToken"></param>
    /// <returns></returns>
    private bool ValidateSessionToken(string sHeaderToken, string sSessionToken, JwtSecurityToken oJwtSecurityToken)
    {
        if (String.Compare(sHeaderToken, sSessionToken) != 0)
        {
            return false;
        }
        return !(oJwtSecurityToken.HasExpired());
    }

    private bool ValidateTokenClaims(string sHeaderToken, AuthorizationFilterContext context, JwtSecurityToken oJwtSecurityToken)
    {
        ////token expiration
        if (context.HttpContext.Items[Constants.Keys.Context.User] == null)     // if token has expired, context[User] will be set to null
        {
            return false;
        }

        //verify that claim was set @ current application
        var oClaim = oJwtSecurityToken.Claims.FirstOrDefault(oThisClaim => String.Compare(oThisClaim.Type, Constants.Claims.ApplicationID, true) == 0);
        if(oClaim == null)
        {
            this.ErrorMsg = "Missing Claim  - Application ID";
            return false;
        }

        if(String.Compare(oClaim.Value, Constants.Claims.ApplicationValue, false) != 0)
        {

            this.ErrorMsg = "Invalid Application ID";

            return false;
        }

        //save header token  @ current session's context
        context.HttpContext.Session.SetString(Constants.Keys.Session.UserToken, sHeaderToken);  
        return true;

    }
}