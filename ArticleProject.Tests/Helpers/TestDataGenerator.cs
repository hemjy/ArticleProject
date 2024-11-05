using ArticleProject.Domain;
using ArticleProject.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticleProject.Tests.Helpers
{
    internal class TestDataGenerator
    {
        public static List<Article> Articles() {
            return new List<Article> {
                    new () { Title = "article one" },
                    new () { Title = "article Two" },
                    new () { Title = "article Three" },
                };
            }
        public static List<User> Users()
        {
            return new List<User> {
                new() { Email = "admin@arcticle.com" },
                 new() { Email = "user@arcticle.com" },
                 new() { Email = "manager@arcticle.com" }
                 };

        }
        public static void PopulateDb(AppDbContext context)
        {
            if (!context.Articles.Any())
            {
                var items = Articles();

                context.Articles.AddRange(items);
            }
            if (!context.Users.Any())
            {
                var users = Users();
                context.Users.AddRange(users);
            }
           

            context.SaveChanges();
        }
    }
}
