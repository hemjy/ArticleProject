using ArticleProject.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArticleProject.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserArticleLike> UserArticleLikes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the primary key for the UserArticleLikes table
            modelBuilder.Entity<UserArticleLike>()
                .HasKey(ual => new { ual.UserId, ual.ArticleId });
        }
    }
}
