

using ArticleProject.Application.Common;
using ArticleProject.Application.Interfaces.Repositories;
using ArticleProject.Application.Interfaces.Services;
using ArticleProject.Domain;
using MediatR;

namespace ArticleProject.Application.Features.Articles.Commands
{
    public class IncrementLikeCountCommand : IRequest<Response>
    {
        public Guid ArticleId { get; set; }
        public string UserEmail { get; set; }
    }

    public class IncrementLikeCountCommandHandler : IRequestHandler<IncrementLikeCountCommand, Response>
    {
        private readonly IGenericRepositoryAsync<Article> _articleRepository;
        private readonly IGenericRepositoryAsync<User> _userRepository;
        private readonly IGenericRepositoryAsync<UserArticleLike> _userArticleLikeRepository;
        private readonly IDistributedLockService _distributedLockService;

        public IncrementLikeCountCommandHandler(
            IGenericRepositoryAsync<Article> articleRepository,
            IGenericRepositoryAsync<UserArticleLike> userArticleLikeRepository,
            IGenericRepositoryAsync<User> userRepository,
            IDistributedLockService distributedLockService
            )
        {
            _articleRepository = articleRepository;
            _userArticleLikeRepository = userArticleLikeRepository;
            _userRepository = userRepository;
            _distributedLockService = distributedLockService;
        }

        public async Task<Response> Handle(IncrementLikeCountCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserEmail)) return Response.Failure("Email is Required");

            var user = await _userRepository.GetByAsync(x => x.Email.Trim().ToLower() == request.UserEmail.Trim().ToLower() && !x.IsDeleted);
            if (user == null) Response.Failure(" User bot Found");
            // Check if user has already liked the article

            var hasLiked = await _userArticleLikeRepository.IsUniqueAsync( x => x.UserId == user.Id && x.ArticleId == request.ArticleId  && !x.IsDeleted);
            if (hasLiked)
            {
               return Response.Failure("User has already liked this article.");
            }

            // Use a distributed lock to prevent race conditions
            var lockKey = $"article:{request.ArticleId}:likeCount";
            var lockValue = Guid.NewGuid().ToString(); // Unique lock value

            var lockAcquired = await _distributedLockService.AcquireLockAsync(lockKey, lockValue, TimeSpan.FromSeconds(30), cancellationToken);
            if (!lockAcquired)
            {
                return Response.Failure("Unable to acquire lock. Please try again later.");
            }

            try
            {
                // Fetch the article and increment the like count
                var article = await _articleRepository.GetByIdAsync(request.ArticleId);
                if (article == null)
                {
                    return Response.Failure("Article not found.");
                }

                article.Likes++;
                article.LastModified = DateTime.UtcNow;
                await _articleRepository.UpdateAsync(article, false);
                await _userArticleLikeRepository.AddAsync(new UserArticleLike { ArticleId = request.ArticleId, UserId = user.Id }, false);

                // Save the like action to the like table
                await _userArticleLikeRepository.SaveChangesAsync();

            }
            finally
            {
                // Release the distributed lock after operation
                await _distributedLockService.ReleaseLockAsync(lockKey, lockValue);
            }

            return Response.Success();
        }

      
    }

}
