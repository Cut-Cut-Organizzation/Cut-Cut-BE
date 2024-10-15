using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.DAL.DBContext
{
    public class CutCutDbContext: DbContext
    {
        public CutCutDbContext(DbContextOptions<CutCutDbContext> options): base(options) 
        {
        
        }
        
    }
}
