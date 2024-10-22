using AutoMapper;
using Cut_Cut.BLL.DTOs;
using Cut_Cut.DAL.Entities;
using System.Runtime;

namespace Cut_Cut.API.Configuration
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<RegisterDTO, User>().ReverseMap();
        }
    }
}
