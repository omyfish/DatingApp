using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.PhotoUrl, opt 
                => opt.ResolveUsing(s => s.Photos.FirstOrDefault(x => x.IsMain)?.Url))
            .ForMember(dest => dest.Age, opt 
                => opt.ResolveUsing(s => s.DateOfBirth.CalAge()));


            CreateMap<User, UserForDetailDto>()
            .ForMember(dest => dest.PhotoUrl, opt 
                => opt.ResolveUsing(s => s.Photos?.FirstOrDefault(x => x.IsMain)?.Url))
            .ForMember(dest => dest.Age, opt 
                => opt.ResolveUsing(s => s.DateOfBirth.CalAge()));

            CreateMap<Photo, PhotoForDetailDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UserForRegisterDto, User>();
            
            CreateMap<MessageForCreationDto, Message>().ReverseMap();

            CreateMap<Message, MessageToReturnDto>()
            .ForMember(m => m.SenderPhotoUrl, dest => dest.MapFrom(o => o.Sender.Photos.FirstOrDefault(u => u.IsMain).Url))
            .ForMember(m => m.RecipientPhotoUrl, dest => dest.MapFrom(o => o.Recipient.Photos.FirstOrDefault(u => u.IsMain).Url));
        }
    }
}