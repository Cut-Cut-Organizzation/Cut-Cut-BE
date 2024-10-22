using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.DAL.Entities
{
    public class User : IdentityUser
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public String? Address { get; set; }
        public String? HairSalonName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

    }
}
