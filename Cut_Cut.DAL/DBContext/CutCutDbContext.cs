using Cut_Cut.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Cut_Cut.DAL.DBContext
{
    public class CutCutDbContext : IdentityDbContext<User>
    {
        public CutCutDbContext(DbContextOptions<CutCutDbContext> options) : base(options)
        {
        
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().Property(u => u.Address).HasMaxLength(25);
        
        }

    }
}
