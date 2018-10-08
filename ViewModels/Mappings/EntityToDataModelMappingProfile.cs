using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using robstagram.Models.Entities;

namespace robstagram.ViewModels.Mappings
{
    public class EntityToDataModelMappingProfile : Profile
    {
        public EntityToDataModelMappingProfile()
        {
            CreateMap<Entry, PostData>()
                .ForMember(x => x.Id, map => map.MapFrom(x => x.Id))
                .ForMember(x => x.Owner, map => map.MapFrom(x => x.Owner.Identity.FirstName))
                .ForMember(x => x.ImageUrl, map => map.MapFrom(x => "/" + x.Picture.Url))
                .ForMember(x => x.Description, map => map.MapFrom(x => x.Description))
                .ForMember(x => x.Likes, map => map.MapFrom(x => x.Likes.Select(l => l.Customer.Identity.FirstName).ToList()))
                .ForMember(x => x.Comments, map => map.MapFrom(x => x.Comments.Select(c => c.Text).ToList()))
                .ForMember(x => x.DateCreated, map => map.MapFrom(x => x.DateCreated));
        }
    }
}
