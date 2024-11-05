using ArticleProject.Application.Common;
using ArticleProject.Application.Features.Articles.Commands;
using ArticleProject.Application.Features.Articles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArticleProject.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(IMediator mediator, ILogger<ArticlesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // User likes an article (Command)
        [HttpPost("like")]
        public async Task<IActionResult> LikeArticle([FromBody] IncrementLikeCountCommand model, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(model, cancellationToken);
            return response.Succeeded ? Ok(response) : BadRequest(response);


        }

        // Get the like count (Query)
        [HttpGet("{articleId}/like-count")]
        public async Task<IActionResult> GetLikeCount([FromRoute]Guid articleId)
        {
            var query = new GetLikeCountQuery { ArticleId = articleId };
            var response = await _mediator.Send(query);
            return response.Succeeded ? Ok(response) : BadRequest(response);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllArticlesQuery model)
        {
            var response = await _mediator.Send(model);
            return response.Succeeded ? Ok(response) : BadRequest(response);
        }


    }
}
