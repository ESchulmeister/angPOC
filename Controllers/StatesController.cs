using angPOC.Data;
using angPOC.Models;
using angPOC.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace angPOC.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatesController : ControllerBase
    {

        #region Variables
        private readonly IStateRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private static readonly Type? type = typeof(State);
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        private readonly string keyStates = type.FullName;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8601 // Possible null reference assignment.

        #endregion

        #region Constructors
        public StatesController(IStateRepository repository, IMapper mapper, IMemoryCache  memoryCache)
        {
            _repository = repository;
            _mapper = mapper;
            _memoryCache = memoryCache;

        }
        #endregion


        /// <summary>
        /// Cached list of states
        /// </summary>
        /// HTTP GET : /api/states
        /// <returns>
        ///  HTTP 200 - success; JSON Array :
        ///      {
        ///      "id": 0,        
        ///       "Code": "string",
        ///       "Name": "string",
        ///     }, 
        ///     ...
        /// ]
        /// OR HTTP 400/500 w/ error/validation message(s)
        /// </returns>
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<StateModel>>> GetStates()
        {
            IEnumerable<StateModel>? results = null;

            // If found in cache, return cached data
            if (_memoryCache.TryGetValue(keyStates, out results))
            {
                return this.Ok(results);
            }

            // If not found, then calculate response
            var lstStates = await _repository.GetStates();

            results = _mapper.Map<IEnumerable<State>, IEnumerable<StateModel>>((lstStates));

            // Set cache options 
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(600),
                Priority = CacheItemPriority.High,
                // cache will expire if inactive for 2 minutes
              //  SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            //// Set object in cache
            _memoryCache.Set(keyStates, results, cacheOptions);

            return this.Ok(results);
        }

  
    }
}
