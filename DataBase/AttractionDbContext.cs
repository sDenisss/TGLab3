using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TG.Attractions;

namespace TG.DataBase
{
    public class AttractionDbContext : DbContext
    {

        public AttractionDbContext(DbContextOptions<AttractionDbContext> options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("DataSource=mydatabase.db; Cache=Shared");
        }

        public DbSet<Attraction>? Attraction { get; set; }
    }
}
