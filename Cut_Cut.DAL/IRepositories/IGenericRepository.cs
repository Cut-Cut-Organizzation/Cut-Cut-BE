using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cut_Cut.DAL.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> FindAsync(string id);
        Task<List<T>> GetAllAsync();
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
