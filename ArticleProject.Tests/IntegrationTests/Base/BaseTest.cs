using ArticleProject.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ArticleProject.Tests.IntegrationTests.Base
{

    public abstract class BaseTest<TController> : IDisposable where TController : ControllerBase
    {
        protected readonly ArticleApplication<TController> _application;
        protected readonly HttpClient _client;
        protected BaseTest()
        {
            _application = new ArticleApplication<TController>();
            _client = _application.CreateClient();
        }
        public virtual void Dispose()
        {
            _client.Dispose();
            _application.Dispose();
        }

    }
}
