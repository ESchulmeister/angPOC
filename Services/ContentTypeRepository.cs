using angPOC.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace angPOC.Services
{

    public class ContentTypeRepository : IContentTypeRepository
    {
        #region Variables

        private readonly UserDBContext _context;

        #endregion

        #region Constructors
        public ContentTypeRepository(UserDBContext context)
        {
            _context = context;
        }

     
        #endregion

        #region Methods
        
        /// <summary>
        /// Get List of content types 
        /// </summary>
        public async Task<IEnumerable<ContentType>> GetContentTypesAsync()
        {
            IQueryable<ContentType> query = _context.ContentTypes;

            return await query.ToListAsync();
        }


        /// <summary>
        /// Content Type By ID 
        /// </summary>
        /// <param name="ID">Content Type ID</param>
        public async Task<ContentType> GetContentTypeAsync(int id)
        {
            return await _context.ContentTypes
                .FirstOrDefaultAsync(x => x.CntId == id);
        }


        /// <summary>
        ///Delete content type 
        /// <param name="ID">Content Type ID - to be deleted</param>
        /// </summary>
        public async  Task DeleteContentTypeAsync(int id)
        {
            var result = await _context.ContentTypes
                .FirstOrDefaultAsync(e => e.CntId == id);
         
 
            _context.ContentTypes.Remove(result);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Add New ContentType
        /// </summary>
        /// <param name="repoContentType">
        /// Body - raw  JSON
        /// </param>
        /// <returns>JSON :
        ///     {
        ///          "id": new id,
        ///           "name": "string"
        ///      }
        /// </returns>   
        public async Task AddContentTypeAsync(ContentType repoContentType)
        {
            _context.ContentTypes.Add(repoContentType);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Dupe check
        /// </summary>
        /// <param name="name"></param>
        public bool ContentTypeExists(string name)
        {
            return _context.ContentTypes.Any(e => e.CntName == name);
        }



        #endregion
    }
}
