using AutoMapper;
using MemberWebApp.Controllers;
using MemberWebApp.Entities;


namespace MemberWebApp.Models
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<User, CreateUserModel>().ReverseMap();
            CreateMap<User, EditUserModel>().ReverseMap();

            CreateMap<User, MemberUserModel>().ReverseMap();
            CreateMap<User, MemberCreateUserModel>().ReverseMap();
            CreateMap<User, MemberEditUserModel>().ReverseMap();


        }
    }
}
