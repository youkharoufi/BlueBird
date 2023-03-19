using AutoMapper;
using BlueBirds.Models;

namespace BlueBirds.AutoMapperProfiles
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {

            CreateMap<RegisterUser, AppUser>();



        }
    }
}
