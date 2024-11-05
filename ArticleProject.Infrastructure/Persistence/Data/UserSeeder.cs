

using ArticleProject.Domain;
using ArticleProject.Infrastructure.Persistence.Context;
using Serilog;

namespace ArticleProject.Infrastructure.Persistence.Data
{
    internal class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();
            SeedUsers(context);
            SeedArticles(context);
        }
        private static void SeedUsers(AppDbContext context)
        {
            if (!context.Users.Any())
            {

                var users = new List<User> {
                    new () { Email = "admin@arcticle.com" },
                    new () { Email = "user@arcticle.com" },
                    new () { Email = "manager@arcticle.com" }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }

           
        }

        private static void SeedArticles(AppDbContext context)
        {
            if (!context.Articles.Any())
            {

                var items = new List<Article> {
                    new () { Title = "article one" },
                    new () { Title = "article Two" },
                    new () { Title = "article Three" },
                };

                context.Articles.AddRange(items);
                context.SaveChanges();
            }
        }
    }

    
}
