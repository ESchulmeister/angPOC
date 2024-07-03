using AutoMapper;
using angPOC.Data;
using angPOC.Models;

namespace angPOC
{

    /// <summary>
    /// Data mapping between 
    /// State Entity - Data/State.cs  
    /// AND
    /// State Model - /Models/StateModel.cs 
    /// </summary>
    public class StateProfile :Profile
    {

        public StateProfile() => this.CreateMap<State, StateModel>()
                .ForMember(c => c.ID, o => o.MapFrom(c => c.Id))
                .ForMember(c => c.Code, o => o.MapFrom(c => c.Code))
                .ForMember(c => c.Name, o => o.MapFrom(c => c.Name))

                .ReverseMap();
    }
}
