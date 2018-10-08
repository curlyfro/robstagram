using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using robstagram.Models.Entities;

namespace robstagram.ViewModels.Mappings
{
    public class DataModelToEntityMappingProfile : Profile
    {
        public DataModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, AppUser>()
                .ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));            
        }
    }
}
