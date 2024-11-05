using ArticleProject.Application.Common;
using ArticleProject.Application.DTOs.Articles;
using ArticleProject.Application.Interfaces.Repositories;
using ArticleProject.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ArticleProject.Application.Features.Articles.Queries
{
    public class GetAllArticlesQuery : RequestParameter, IRequest<PagedResponse<IReadOnlyList<GetArticleDto>>>
    {
    }

    public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, PagedResponse<IReadOnlyList<GetArticleDto>>>
    {
        private readonly IGenericRepositoryAsync<Article> _articleRepository;

        public GetAllArticlesQueryHandler(IGenericRepositoryAsync<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<PagedResponse<IReadOnlyList<GetArticleDto>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {

            var articleQuery =  _articleRepository.GetAllQuery().Where(x => !x.IsDeleted);

            var skip = (request.PageNumber - 1) * request.PageSize;
            var articleLikeCount = await articleQuery.Take(request.PageSize).Skip(skip).Select(a => new GetArticleDto
            {
                Id = a.Id,
                LikeCount = a.Likes,
                Title = a.Title
            }).ToListAsync(cancellationToken);

            var total = await articleQuery.CountAsync(cancellationToken);
            return new PagedResponse<IReadOnlyList<GetArticleDto>>(articleLikeCount, request.PageNumber, request.PageSize, total) { Message = $"{total} record(s) found." };
           
        }
    }

}
