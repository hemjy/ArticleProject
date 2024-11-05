using ArticleProject.Application.Common;
using ArticleProject.Application.DTOs.Articles;
using ArticleProject.Application.Interfaces.Repositories;
using ArticleProject.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ArticleProject.Application.Features.Articles.Queries
{
    public class GetLikeCountQuery : IRequest<Response<GetArticleDto>>
    {
        public Guid ArticleId { get; set; }
    }

    public class GetLikeCountQueryHandler : IRequestHandler<GetLikeCountQuery, Response<GetArticleDto>>
    {
        private readonly IGenericRepositoryAsync<Article> _articleRepository;

        public GetLikeCountQueryHandler(IGenericRepositoryAsync<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<Response<GetArticleDto>> Handle(GetLikeCountQuery request, CancellationToken cancellationToken)
        {

            var articl = await _articleRepository.GetAllQuery().Where(x => x.Id == request.ArticleId && !x.IsDeleted).Select(x => 
            new GetArticleDto
            {
                Title = x.Title,
                Id = x.Id,
                LikeCount = x.Likes
            }).FirstOrDefaultAsync(cancellationToken);



            return Response<GetArticleDto>.Success(articl ?? new());
           
        }
    }

}
