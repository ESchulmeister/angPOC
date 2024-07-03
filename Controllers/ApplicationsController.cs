using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using angPOC.Data;
using angPOC.Models;
using angPOC.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace angPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {

        #region Variables
        private readonly IApplicationRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;


        #endregion

        #region Constructors

        public ApplicationsController(IApplicationRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        #endregion


        #region Methods

        /// <summary>
        /// Get Applications
        /// </summary>
        /// HTTP GET : /api/applications
        /// <param name="id">User ID</param>
        ///<returns>
        /// HTTP 200 - success &  JSON Array
        /// [
        ///      {
        ///        "id": "number",
        ///         "name": "string",
        ///         "isActive": "boolean",
        ///         "createdDate": "2021-10-29T17:00:26.703",
        ///         "createdBy": "string",
        ///         "modifiedDate": "2021-10-29T17:00:26.703"
        ///     },
        ///     ...
        ///    ]
        /// OR HTTP 400/500 w/ error/validation message(s)
        /// </returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationModel>>> GetAllApplications()
        {
            IEnumerable<Application> results = await _repository.GetApplicationsAsync();

            return Ok(_mapper.Map<ApplicationModel[]>(results));
        }


        /// <summary>
        /// Update application
        /// </summary>
        /// HTTP PUT :/api/applications/{id}
        /// <param name="id"></param>
        /// <param name="model">JSON</param>
        /// <returns>
        /// HTTP 200 - success; NoContent
        /// OR  HTTP 400/500 w/ error/validation message(s)
        /// </returns>
        [Authorize]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateApplication(int id, [FromBody] ApplicationModel model)
        {
            if(model.ID == 0)
            {
                model.ID = id;
            }
            
            if (id != model.ID)
            {
                throw new NotFoundException("Cant update Application - ID mismatch");
            }

            try
            {
                Application repo  = _mapper.Map<Application>(model);

                await _repository.SaveApplicationAsync(repo);
            }
            catch (DbUpdateConcurrencyException ) 
            {
                if (!_repository.ApplicationExists(id))
                {
                    throw new DataValidationException($"Invalid application ID - {id.ToString()} ");
                }
                else
                {
                    throw ;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Delete application
        /// </summary>
        /// HTTP DELETE ::/api/applications/{id}
        /// <param name="id"></param>
        /// <returns>
        /// HTTP 200 - success
        /// OR  HTTP 400/500 w/ error/validation message(s)
        /// </returns>
        [Authorize]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var oDeletedApplication = await _repository.GetApplicationAsync(id);
            if (oDeletedApplication == null)
            {
                throw new NotFoundException($"Invalid application ID - {id.ToString()} ");
            }

            await _repository.DeleteApplicationAsync(id);

            return Ok();
        }

        /// <summary>
        /// Update application
        /// </summary>
        /// HTTP PUT : /api/applications
        /// <param name="model">JSON Body</param>
        /// <returns>
        /// HTTP 200 - success/created
        /// OR  HTTP 400/500 w/ error/validation message(s)
        /// </returns>
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> AddApplication([FromBody] ApplicationModel model)
        {

            Application repoApplication = _mapper.Map<Application>(model);

            await _repository.SaveApplicationAsync(repoApplication);

            var newApplication = _mapper.Map<ApplicationModel>(repoApplication);    //new ApplicationModel

            //Route  - GET - uri - newly created Application
            var url = _linkGenerator.GetPathByAction(HttpContext, "Get", values: new { id = newApplication.ID });

            return Created(url, _mapper.Map<ApplicationModel>(newApplication));
        }
        #endregion

    }

}
