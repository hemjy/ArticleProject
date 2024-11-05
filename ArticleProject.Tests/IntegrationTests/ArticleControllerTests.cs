using ArticleProject.Application.Common;
using ArticleProject.Application.DTOs.Articles;
using ArticleProject.Application.Features.Articles.Commands;
using ArticleProject.Infrastructure.Persistence.Context;
using ArticleProject.Presentation.Controllers;
using ArticleProject.Tests.Helpers;
using ArticleProject.Tests.IntegrationTests.Base;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
namespace ArticleProject.Tests.IntegrationTests
{
    public class ArticleControllerTests : BaseTest<ArticlesController>
    {
        private readonly AppDbContext _context;
        public ArticleControllerTests()
        {
            _context = _application.Services.CreateScope().ServiceProvider
                        .GetRequiredService<AppDbContext>();
            TestDataGenerator.PopulateDb(_context);
        }

        [Fact]
        public async Task ShouldReturnArticleWithLikeCount()
        {
            var item = _context.Articles.First();


            var response = await _client
               .GetAsync($"/api/v1/articles/{item.Id}/like-count");
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseDta = JsonConvert.DeserializeObject<Response<GetArticleDto>>(content);
            responseDta.Should().NotBeNull();
            responseDta.Succeeded.Should().BeTrue();
            responseDta.Data.Should().NotBeNull();
            responseDta.Data.Id.Should().Be(item.Id);
            responseDta.Data.LikeCount.Should().BeGreaterThanOrEqualTo(0);
        }

        public async Task ShouldReturnArticleWithLikeCount_WhenLike()
        {
           var item = _context.Articles.First();
            var article = _context.Articles.First();
            var user = _context.Users.First();

            // Arrange
            var payload = new IncrementLikeCountCommand
            {
                ArticleId = article.Id,
                UserEmail = user.Email
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            // Act
           await _client.PostAsync("/api/v1/articles/like", content);

            var response = await _client
               .GetAsync($"/api/v1/articles/{item.Id}/like-count");
            var result = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseDta = JsonConvert.DeserializeObject<Response<GetArticleDto>>(result);
            responseDta.Should().NotBeNull();
            responseDta.Succeeded.Should().BeTrue();
            responseDta.Data.Should().NotBeNull();
            responseDta.Data.Id.Should().Be(item.Id);
            responseDta.Data.LikeCount.Should().BeGreaterThan(article.Likes);
        }

        [Fact]
        public async Task ShouldReturnOk_WhenLike()
        {
            var article = _context.Articles.First();
            var user = _context.Users.First();
            
            // Arrange
            var payload = new IncrementLikeCountCommand
            {
                ArticleId = article.Id,
                UserEmail = user.Email
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/articles/like", content);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            var responseDta = JsonConvert.DeserializeObject<Response>(result);
            responseDta.Should().NotBeNull();
            responseDta.Succeeded.Should().BeTrue();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenLikeMOrethan()
        {
            var article = _context.Articles.First();
            var user = _context.Users.First();

            // Arrange
            var payload = new IncrementLikeCountCommand
            {
                ArticleId = article.Id,
                UserEmail = user.Email
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            // Act
             await _client.PostAsync("/api/v1/articles/like", content);
            var response = await _client.PostAsync("/api/v1/articles/like", content);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            var responseDta = JsonConvert.DeserializeObject<Response>(result);
            responseDta.Should().NotBeNull();
            responseDta.Succeeded.Should().BeFalse();
            responseDta.Message.Should().Be("User has already liked this article.");
        }

    }
}