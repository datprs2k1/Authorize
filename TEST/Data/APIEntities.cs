using Microsoft.EntityFrameworkCore;

namespace TEST.Data
{
    public class APIEntities : DbContext
    {
        public APIEntities(DbContextOptions options) : base(options) { }

        #region DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<Token>(e =>
            {
                e.ToTable("tokens");
                e.HasOne(e => e.User)
                .WithMany(e => e.Tokens)
                .HasForeignKey(e => e.UserID);
            });
        }
    }
}
