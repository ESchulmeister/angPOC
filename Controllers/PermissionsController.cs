using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using angPOC.Data;
using angPOC.Models;
using angPOC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;

namespace angPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {

        #region Variables
        private readonly IPermissionRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        #endregion

        #region Constructor
        public PermissionsController(IPermissionRepository repository, IMapper mapper, LinkGenerator  linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;

        }
        #endregion


        #region Methods

        /// <summary>
        /// Get - List<PermissionModel>
        /// </summary>
        /// HTTP GET : /api/permissions?appID={id}
        /// <param name="appID">application ID</param>
        /// <returns>HTTP 200 success & JSON array:
        /// [
        ///    {
        ///        "id": number,
        ///        "name": string
        ///    }
        ///    [, ....]
        /// ]
        ///  OR HTTP 400/500 w/ error/validation message
        ///</returns>
        /// </returns>
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<PermissionModel>>> GetApplicationPermissions([FromQuery] int appID)
        {
            IEnumerable<AppPermission> results  = await _repository.GetApplicationPermissions(appID);

            return Ok(_mapper.Map<PermissionModel[]>(results));
        }

        /// <summary>
        /// Add permission
        /// </summary>
        /// HTTP POST :/api/permissions?appID={id}
        /// <param name="appID"></param>
        /// <param name="model">JSON
        ///  {
        ///        "id": number,
        ///        "name": string
        ///    }     
        /// </param>
        ///<returns>
        /// HTTP 200 - success
        /// OR HTTP 400/500 w/ error/validation message(s)
        ///</returns>      
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> AddPermission([FromQuery] int appID, [FromBody] PermissionModel model)
        {
            try
            {

                AppPermission repoPermission = _mapper.Map<AppPermission>(model);
                await _repository.SavePermissionAsync(appID, repoPermission);

                var newPermission = _mapper.Map<PermissionModel>(model);

                //Route  - GET - uri - newly created user PermissionModel
                var url = _linkGenerator.GetPathByAction(HttpContext, "Get", values: new { id = newPermission.ID });

                return Created(url, _mapper.Map<PermissionModel>(newPermission));
            }

            catch (DbUpdateConcurrencyException)
            {
                throw new DataValidationException();
            }
        }


   
        #endregion

    }

}
