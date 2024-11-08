﻿namespace ArticleProject.Domain
{
    public class UserArticleLike : EntityBase
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
