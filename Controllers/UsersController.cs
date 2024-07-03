using angPOC.Data;
using angPOC.Models;
using angPOC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace angPOC.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        #region Variables
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        #endregion

        #region Constructors
        public UsersController(IUserRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        #endregion

        /// <summary>
        /// Get User By ID
        /// </summary>
        /// HTTP GET : /api/users/{id}
        /// <param name="id">User ID</param>
        /// <returns>
        /// HTTP 200 - success; JSON  :
        /// {
        ///  "id": number ,
        ///  "login":  string ,
        ///  "lastName":  string ,
        ///  "firstName":  string ,
        ///  "clock": string ,
        ///  "email":  string e.g. user@example.com ,
        ///  "createDate": date - e.g: 2021-09-30T19:02:25.345Z,
        ///  "updateDate": date,
        ///  "isActive": boolean,
        ///  "modifiedBy":  string 
        /// } 
        /// OR HTTP 400/500 w/ error/validation message(s)
        /// </returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> Get(int id)
        {
            if (!_repository.UserExists(id))
            {
                throw new DataValidationException($"Invalid user ID - {id.ToString()} ");
            }

            var result = await _repository.GetUserAsync(id);

            return Ok(_mapper.Map<UserModel>(result));

        }

        /// <summary>
        /// Get List of Users per application (id)
        /// </summary>
        /// HTTP GET : /api/users?appID={appID}
        /// <param name="id">User ID</param>
        /// <returns>JSON  Array:
        /// [
        ///      {
        ///      "id": number,        
        ///       "lastName": string,
        ///       "firstName": string,
        ///       "clock": 4 char. string,
        ///       "email": string e.g; user@example.com,
        ///       "stateID": number,
        ///       "createDate": date, e.g. 2021-09-30T19:02:25.345Z ,
        ///        "updateDate": date,
        ///        "isActive": boolean,
        ///        "modifiedBy": string
        ///     }
        /// ]
        /// OR HTTP 400/500 w/ error/validation message
        /// </returns>
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetByApplication([FromQuery] int appID)
        {
            IEnumerable<User>? results = null;

            if (appID > 0)
            {

                results = await _repository.GetUsersByAppAsync(appID);
            }
            else
            {
                results = await _repository.GetUsersAsync();
            }        

            return  Ok(_mapper.Map<UserModel[]>(results));
        }



        /// <summary>
        /// Add New User
        /// </summary>
        ///  HTTP POST : /api/users
        /// <param name="model">JSON body</param>
        /// <returns>HTTP 201 - success/created
        /// and New User JSON
        ///  OR HTTP 400/500 w/ error/validation message
        ///</returns>
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> AddUser([FromBody] UserModel model)
        {
            try
            {

                User repoUser = _mapper.Map<User>(model);

                await _repository.SaveUserAsync(repoUser);

                var newUser = _mapper.Map<UserModel>(model);

                //Route  - GET - uri - newly created user model
                var url = _linkGenerator.GetPathByAction(HttpContext, "Get", values: new { id = newUser.ID });

                return Created(url, _mapper.Map<ApplicationModel>(newUser));

            }

            catch (DbUpdateConcurrencyException)
            {

                throw new DataValidationException();

            }



        }

        /// <summary>
        /// Update User
        /// </summary>
        /// HTTP PUT: /api/users?appID={appID}
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Updated user JSON:
        //     {
        ///       "id": 0,        
        ///       "lastName": "string",
        ///       "firstName": "string",
        ///       "clock": "string",
        ///       "email": "user@example.com",
        ///       "createDate": "2021-09-30T19:02:25.345Z",
        ///        "updateDate": "2021-09-30T19:02:25.345Z",
        ///        "isActive": true,
        ///        "modifiedBy": "string"
        ///     }
        ///  OR HTTP 400/500 -  error/validation message(s)
        /// </returns>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel model)
        {
            if (id != model.ID)
            {
                throw new DataValidationException($"Cannot update user - ID mismatch- [{ id.ToString()}]");
            }

            try
            {
                User repoUser = _mapper.Map<User>(model);

                await _repository.SaveUserAsync(repoUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repository.UserExists(id))
                {
                    throw new DataValidationException($"Invalid user ID - {id.ToString()} ");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
    
        /// <summary>
        /// Activate/Reactivate user - reset active flag - partial update
        /// </summary>
        /// HTTP PATCH :/api/users/{id}
        /// <param name="id">User Id</param>
        /// <param name="patchDocument">JSON arrray - values to replace. :
        ///  [
        ///   {
        ///         "value": "false",
        ///         "path": "/IsActive",
        ///         "op": "replace"
        ///   }, ...
        ///]
        /// </param>
        /// <returns>HTTP 204 - No Content
        ///  OR HTTP 400/500 w/ error/validation message(s)</returns>
        [Authorize]
        [HttpPatch]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] JsonPatchDocument<UserModel> patchDocument)
        {
            if (!_repository.UserExists(id))
            {
                throw new BadRequestException($"Invalid user ID - {id.ToString()} ");
            }

            var repoUser = await _repository.GetUserAsync(id);
            var userToPatch = _mapper.Map<UserModel>(repoUser);            
            patchDocument.ApplyTo(userToPatch, ModelState);        
            
            if (!TryValidateModel(userToPatch))
            {
                return ValidationProblem(ModelState);    //BadRequest
            }

            _mapper.Map(userToPatch, repoUser);
            await _repository.UpdateUserAsync(repoUser);
            return NoContent();
        }

        public override ActionResult ValidationProblem(
         [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

   
        /// <summary>
        /// HTTP POST : http:../api/user/logout
        /// </summary>
        /// Reset session jwt token
        /// <returns></returns>
        [HttpPost("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove(Constants.Keys.Session.UserToken);

            return Ok();
        }

    }
}
