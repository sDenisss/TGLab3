using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TG.Users;

namespace TG.DataBase
{
    public class UserDbContext : DbContext
    {

        public UserDbContext(DbContextOptions<UserDbContext> options)
        {

        }

        private string _connectionString = "Data Source=mydatabase.db";
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("DataSource=mydatabase.db; Cache=Shared");
        }

        public DbSet<User>? User { get; set; }
    }
}