using Cut_Cut.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<Response<TokenResponse>> Login(LoginDTO login);
        Task<Response<bool>> Logout(string tokenDTO);
        Task<Response<TokenResponse>> RefreshToken(RefreshTokenDTO refreshTokenDTO);
    }
}
