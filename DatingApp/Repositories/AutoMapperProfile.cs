using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            string username = null;
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url));
            CreateMap<User, UserDetailedDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url));
            CreateMap<Photo, PhotoDetailedDto>();

            CreateMap<UserRegisterDto, User>().ReverseMap();

            CreateMap<UserForUpdateDto, User>();

            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => 
                src.Sender.Photos.Where(p => p.IsMain == true).Select(p => p.Url).FirstOrDefault()))
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src =>
                src.Recipient.Photos.Where(p => p.IsMain == true).Select(p => p.Url).FirstOrDefault()))
                .ForMember(dest => dest.UnreadMessageCount, opt => opt.MapFrom(src =>
                src.Recipient.MessagesReceived.Where(m => m.DateRead == null && m.RecipientUsername == username).Count()));

            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

        }
    }
}
