using ArticleProject.Application.Common;
using ArticleProject.Application.Features.Auth.Helpers;
using ArticleProject.Application.Interfaces.Repositories;
using ArticleProject.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ArticleProject.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<string>>
    {
        private readonly IGenericRepositoryAsync<User> _userRepository;
        private readonly IConfiguration _config;

        public LoginCommandHandler(
           IGenericRepositoryAsync<User> userRepository,
            IConfiguration config
            )
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<Response<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email)) return Response<string>.Failure("Email is Required");
            if (string.IsNullOrWhiteSpace(request.Password)) return Response<string>.Failure("Password is Required");
           
            var user = await _userRepository.GetByAsync(x => x.Email.Trim().ToLower() == request.Email.Trim().ToLower() && !x.IsDeleted);
            if (user == null) throw new UnauthorizedAccessException("Invalid Email/Password");
         var token =   AuthHelper.GenerateJwtToken(user.Id, user.Email, _config);

            return Response<string>.Success(token);
        }


    }


}
