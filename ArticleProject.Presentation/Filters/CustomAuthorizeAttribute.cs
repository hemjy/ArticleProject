using ArticleProject.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArticleProject.Presentation.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var path = context.HttpContext.Request.Path.ToString().ToLower();
            var ret = IsAllowAnonymous(context);
            bool isUnauthorised = !context.HttpContext.User.Identity.IsAuthenticated && !IsAllowAnonymous(context);
            bool isUnauthorisedd = !context.HttpContext.User.Identity.IsAuthenticated && !ret;

            if (isUnauthorised)
            {
                throw new UnauthorizedAccessException("You are not authorized.");
            }

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var descriptors = context.ActionDescriptor.EndpointMetadata.ToList();

                var attribute = (AuthorizeAttribute)descriptors.FirstOrDefault(x => x.ToString().Contains("Authorize"));

                if (attribute?.Policy != null && !context.HttpContext.User.IsInRole(attribute.Policy))
                {
                    throw new HttpBadForbiddenException("You are not allowed to access this resource");
                }
            }
        }

        private static bool IsAllowAnonymous(AuthorizationFilterContext context)
        {
            // Check if the action is decorated with AllowAnonymous attribute
            return context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
        }
    }
}
