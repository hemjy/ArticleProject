using ArticleProject.Application.Features.Articles.Commands;
using ArticleProject.Application.Interfaces.Repositories;
using ArticleProject.Application.Interfaces.Services;
using ArticleProject.Domain;
using Moq;

namespace ArticleProject.Tests.UnitTests.Articles
{
    public class IncrementLikeCountCommandHandlerTests
    {
        private readonly Mock<IGenericRepositoryAsync<User>> _mockUserRepository;
        private readonly Mock<IGenericRepositoryAsync<UserArticleLike>> _mockUserArticleLikeRepository;
        private readonly Mock<IDistributedLockService> _mockDistributedLockService;
        private readonly Mock<IGenericRepositoryAsync<Article>> _mockArticleRepository;
        private readonly IncrementLikeCountCommandHandler _handler;

        public IncrementLikeCountCommandHandlerTests()
        {
            _mockUserRepository = new Mock<IGenericRepositoryAsync<User>>();
            _mockUserArticleLikeRepository = new Mock<IGenericRepositoryAsync<UserArticleLike>>();
            _mockDistributedLockService = new Mock<IDistributedLockService>();
            _mockArticleRepository = new Mock<IGenericRepositoryAsync<Article>>();
            _handler = new IncrementLikeCountCommandHandler(
                 _mockArticleRepository.Object,
                  _mockUserArticleLikeRepository.Object,
                _mockUserRepository.Object,
                _mockDistributedLockService.Object
               
            );
        }

        [Fact]
        public async Task Handle_EmailIsNullOrEmpty_ShouldReturnFailure()
        {
            // Arrange
            var command = new IncrementLikeCountCommand
            {
                UserEmail = string.Empty, // Invalid email
                ArticleId = Guid.NewGuid(),
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal("Email is Required", result.Message);
        }

       
        [Fact]
        public async Task Handle_UserAlreadyLikedArticle_ShouldReturnFailure()
        {
            // Arrange
            var command = new IncrementLikeCountCommand
            {
                UserEmail = "user@example.com",
                ArticleId = Guid.NewGuid()
            };

            var user = new User { Id = Guid.NewGuid(), Email = "user@example.com" };
            _mockUserRepository.Setup(repo => repo.GetByAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                .ReturnsAsync(user);

            _mockUserArticleLikeRepository.Setup(repo => repo.IsUniqueAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserArticleLike, bool>>>()))
                .ReturnsAsync(true); // Simulate that the user has already liked the article

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal("User has already liked this article.", result.Message);
        }

      

    }
}
