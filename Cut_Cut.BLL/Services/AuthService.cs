using Cut_Cut.BLL.DTOs;
using Cut_Cut.BLL.Interfaces;
using Cut_Cut.DAL.Entities;
using Cut_Cut.DAL.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public AuthService(UserManager<User> userManager,RoleManager<IdentityRole> roleManager,IConfiguration configuration) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<Response<TokenResponse>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                }
                var token = GetToken(authClaims);
                var refreshToken = GetRefreshToken();
                var isRefreshTokenSaved = await SaveRefreshToken(user, refreshToken);
                var tokenResponse = new TokenResponse
                {
                    AccessToken = "Bearer " + new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                };

                return new Response<TokenResponse>() { Data = tokenResponse , StatusCode = System.Net.HttpStatusCode.OK};
                
            }
            return new Response<TokenResponse>() { StatusCode = System.Net.HttpStatusCode.Unauthorized, Message = "Failed to Check Password for this user!"};
        }

        

        public async Task<Response<bool>> Logout(string tokenDTO)
        {
            try
            {

                var resutlEmailClaim = EmailClaim(tokenDTO);
                var user = await _userManager.FindByEmailAsync(resutlEmailClaim);
                if (user == null) return new Response<bool>()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Data = false,
                    Message = "Error no User found with the refresh token given"
                };
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                var res = await _userManager.UpdateAsync(user);
                if (res.Succeeded) return new Response<bool>()
                {
                    Data = res.Succeeded,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return new Response<bool>()
                {
                    Data = res.Succeeded,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new Response<bool>() { StatusCode = System.Net.HttpStatusCode.InternalServerError, Message = ex.Message };           
            }
        }

        private string EmailClaim(string tokenDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(tokenDTO) as JwtSecurityToken;
            if (jsonToken != null)
            {
                var emailClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

                if (emailClaim != null)
                {
                    return emailClaim;
                }
                else
                {
                   throw new Exception("No Email Found in the JWT Token");
                }
            }
            else
            {
                throw new Exception("Not Valid Token.");
            }
        }

        public async Task<Response<TokenResponse>> RefreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            if (refreshTokenDTO.RefreshToken == null) return new Response<TokenResponse>() { StatusCode = System.Net.HttpStatusCode.BadRequest, Message = "Error, No Refresh Token given" };
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshTokenDTO.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now) return new Response<TokenResponse>() { StatusCode = System.Net.HttpStatusCode.BadRequest, Message = "Error, User not found or Refresh Token already expired" };
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));

            }
            var newToken = GetToken(authClaims);
            var tokenResponse = new TokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newToken),
                RefreshToken = refreshTokenDTO.RefreshToken,
                Expiration = newToken.ValidTo
            };
            return new Response<TokenResponse>() { StatusCode = System.Net.HttpStatusCode.OK, Data = tokenResponse };

        }

        private async Task<bool> SaveRefreshToken(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryInDays"]));
            var res = await _userManager.UpdateAsync(user);
            return res.Succeeded;


        }

        private string GetRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
       
}

