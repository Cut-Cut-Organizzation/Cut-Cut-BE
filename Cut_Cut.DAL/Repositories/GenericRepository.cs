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
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly CutCutDbContext _context;
        private protected DbSet<T> _dbSet;
        public GenericRepository(CutCutDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                _context.Attach(entity).State = EntityState.Added;

                if (_context.Entry(entity).State == EntityState.Added)
                {
                    await _context.SaveChangesAsync();
                    return entity;
                }
                else
                {
                    throw new InvalidOperationException("Impossible to retrive the ID from th entity");
                }
            }
            catch
            {
                throw;
            }

        }

        //private Guid GetEntityId(T entity)
        //{

        //    var property = entity.GetType().GetProperty("Id");
        //    if (property != null)
        //    {
        //        return (Guid)property.GetValue(entity);
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("Impossible to recover a ID property for this entity.");
        //    }
        //}



        public async Task<List<T>> FindAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch
            {
                throw;
            }

        }

        public async Task<T?> FindAsync(string id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch
            {
                throw;
            }

        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                var result = _dbSet.Update(entity);

                return await _context.SaveChangesAsync() > 0 ? true : false;
            }
            catch
            {
                throw;
            }

        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Deleted;
                var result = await _context.SaveChangesAsync();
                return result > 0;

            }
            catch
            {
                throw;
            }

        }

        public Task<List<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
