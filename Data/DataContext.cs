using antvk_api.Model;
using Microsoft.EntityFrameworkCore;

namespace antvk_api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<place> place { get; set; }
        public DbSet<events> events { get; set; }

    }
}