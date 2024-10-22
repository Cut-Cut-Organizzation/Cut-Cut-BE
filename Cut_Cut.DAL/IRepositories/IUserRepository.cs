using Cut_Cut.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.DAL.IRepositories
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<bool> UpdateUser(User user);
    }
}
