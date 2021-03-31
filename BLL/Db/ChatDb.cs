using IBLL.Models.ChatDb;
using IBLL.Models.UserDb;
using Microsoft.EntityFrameworkCore;

namespace BLL.Db
{
    public class ChatDb : DbContext
    {
        public ChatDb(DbContextOptions<ChatDb> options) : base(options) { }

        public DbSet<ChatGroupMdl> ChatGroups { get; set; }

        public DbSet<ChatGroupUserMdl> ChatGroupUsers { get; set; }

        public DbSet<ChatMessageMdl> ChatMessages { get; set; }

        public DbSet<UserMdl> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ChatGroupUserMdl>()
                .HasKey(p => new { p.UserId, p.GroupId, p.OwnId });

            modelBuilder
                .Entity<UserMdl>()
                .ToSqlQuery("select * from UserDb.dbo.Users");
        }
    }
}