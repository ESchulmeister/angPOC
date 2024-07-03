using AutoMapper;
using angPOC.Data;
using angPOC.Models;

namespace angPOC
{

    /// <summary>
    /// Data mapping between 
    /// ApPermission Entity - Data/AppPermission.cs  
    /// AND
    /// Permission Model - /Models/PermissionModel.cs 
    /// </summary>
    public class PermissionProfile :Profile
    {

        public PermissionProfile() => this.CreateMap<AppPermission, PermissionModel>()
                .ForMember(c => c.ID, o => o.MapFrom(c => c.ApId))
                .ForMember(c => c.Name, o => o.MapFrom(c => c.PermName))
                .ReverseMap();
    }
}
