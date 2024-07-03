using AutoMapper;
using angPOC.Data;
using angPOC.Models;

namespace angPOC
{

    /// <summary>
    /// Data mapping between 
    /// User Entity - Data/User.cs  
    /// AND
    /// User Model - /Models/UserModel.cs 
    /// </summary>
    public class UserProfile :Profile
    {

        public UserProfile() => this.CreateMap<User, UserModel>()
                .ForMember(c => c.ID, o => o.MapFrom(c => c.UsrId))
                .ForMember(c => c.UserName, o => o.MapFrom(c => c.UsrLogin))
                .ForMember(c => c.FirstName, o => o.MapFrom(c => c.UsrFirstName))
                .ForMember(c => c.LastName, o => o.MapFrom(c => c.UsrLastName))
                .ForMember(c => c.Clock, o => o.MapFrom(c => c.UsrClock))
                .ForMember(c => c.Email, o => o.MapFrom(c => c.UsrEmail))
                .ForMember(c => c.IsActive, o => o.MapFrom(c => c.UsrActive))
                .ForMember(c => c.stateID, o => o.MapFrom(c => c.UsrStateId))
                .ForMember(c => c.CreateDate, o => o.MapFrom(c => c.UsrCreatedDate))
                .ForMember(c => c.CreatedBy, o => o.MapFrom(c => c.UsrCreatedBy))
                .ForMember(c => c.UpdateDate, o => o.MapFrom(c => c.UsrModifiedDate))
                .ForMember(c => c.ModifiedBy, o => o.MapFrom(c => c.UsrModifiedBy))
                .ReverseMap();
    }
}
