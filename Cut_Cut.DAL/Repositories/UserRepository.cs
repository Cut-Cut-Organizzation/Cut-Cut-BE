using Cut_Cut.DAL.DBContext;
using Cut_Cut.DAL.Entities;
using Cut_Cut.DAL.IRepositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<User> AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
