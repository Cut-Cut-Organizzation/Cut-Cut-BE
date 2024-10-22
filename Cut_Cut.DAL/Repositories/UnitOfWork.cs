using Cut_Cut.DAL.DBContext;
using Cut_Cut.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CutCutDbContext _context;

      

        public UnitOfWork(CutCutDbContext context)
        {
            _context = context;
           
           
        }



        public async Task<int> Save()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
