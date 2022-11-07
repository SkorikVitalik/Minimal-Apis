using Microsoft.EntityFrameworkCore;

namespace Minimal_Apis.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Hotel> Hotels => Set<Hotel>(); 
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        
    }
}
