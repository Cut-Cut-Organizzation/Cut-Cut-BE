using AutoMapper;
using Cut_Cut.BLL.DTOs;
using Cut_Cut.BLL.Interfaces;
using Cut_Cut.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<Response<RegisterDTO>> AddUser(RegisterDTO registerDTO)
        {
            var userExist = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (userExist == null)
            {
                var newUser = _mapper.Map<User>(registerDTO);
                newUser.UserName = registerDTO.Email;
                var res = await _userManager.CreateAsync(newUser, registerDTO.Password);
                var userToSend = _mapper.Map<RegisterDTO>(newUser);
                if (!res.Succeeded)
                    return new Response<RegisterDTO>() { Message = "Failed during create User", StatusCode = System.Net.HttpStatusCode.InternalServerError };
                return new Response<RegisterDTO>() { StatusCode = System.Net.HttpStatusCode.OK, Data = userToSend};
            }
            return new Response<RegisterDTO>() { Message = "Failed to Add this user, because already exist!", StatusCode = System.Net.HttpStatusCode.BadRequest };

        }
    }
}
