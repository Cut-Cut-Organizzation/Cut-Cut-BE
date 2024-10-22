using Cut_Cut.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.BLL.Interfaces
{
    public interface IUserService
    {
        Task<Response<RegisterDTO>> AddUser(RegisterDTO registerDTO);
    }
}
