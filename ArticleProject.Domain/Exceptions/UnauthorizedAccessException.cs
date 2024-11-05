

namespace ArticleProject.Domain.Exceptions
{
    public class HttpBadForbiddenException : Exception
    {
        public HttpBadForbiddenException(string message) : base(message) { }
    }
}
