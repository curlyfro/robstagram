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
                //.ForMember(x => x.Comments, map => map.MapFrom(e => e.Comments.Select(x => Mapper.Map<Comment, CommentData>(x)).ToList()))
                .ForMember(x => x.Comments, map => map.MapFrom(e => e.Comments))
                .ForMember(x => x.DateCreated, map => map.MapFrom(x => x.DateCreated));

            CreateMap<Customer, ProfileData>()
                .ForMember(x => x.FirstName, map => map.MapFrom(x => x.Identity.FirstName))
                .ForMember(x => x.LastName, map => map.MapFrom(x => x.Identity.LastName))
                .ForMember(x => x.PictureUrl, map => map.MapFrom(x => "/" + x.Identity.PictureUrl))
                //.ForMember(x => x.FacebookId, map => map.MapFrom(x => x.Identity.FacebookId.Value))
                .ForMember(x => x.Location, map => map.MapFrom(x => x.Location))
                .ForMember(x => x.Locale, map => map.MapFrom(x => x.Locale))
                .ForMember(x => x.Gender, map => map.MapFrom(x => x.Gender));

            CreateMap<Comment, CommentData>()
                .ForMember(x => x.Id, map => map.MapFrom(x => x.Id))
                .ForMember(x => x.Owner, map => map.MapFrom(x => x.Owner.Identity.FirstName))
                .ForMember(x => x.Text, map => map.MapFrom(x => x.Text))
                .ForMember(x => x.DateCreated, map => map.MapFrom(x => x.DateCreated));            
    }
    }
}
