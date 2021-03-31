using IBLL.Models.UserDb;
using Microsoft.EntityFrameworkCore;

namespace BLL.Db
{
    public class UserDb : DbContext
    {
        public UserDb(DbContextOptions<UserDb> options) : base(options) { }

        public DbSet<UserMdl> Users { get; set; }

        public DbSet<LogMdl> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}