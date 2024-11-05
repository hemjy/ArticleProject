namespace ArticleProject.Application.DTOs.Articles
{
    public class GetArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int LikeCount { get; set; }
    }
}
