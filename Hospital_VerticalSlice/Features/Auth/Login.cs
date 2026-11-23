using Azure.Core;
using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace School_VerticalSlice.Features.Auth
{
    public record LoginResponse(string AccessToken, string RefreshToken, string Role, string Name);
    public class LoginCommand : IRequest<Result<LoginResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني لا يمكن أن يكون فارغاً.")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة.");
        }
    }

    public class LoginHandler : IRequestHandler<LoginCommand,Result<LoginResponse>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public LoginHandler(ApplicationDbContext context,ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.AsNoTracking()
                                        .Where(u => u.Email == request.Email)
                                        .Select(u => new
                                        {
                                            User = u,
                                            TeacherId = u.Teacher != null ? (int?)u.Teacher.Id : null,
                                            StudentId = u.Student != null ? (int?)u.Student.Id : null,
                                        })
                                        .FirstOrDefaultAsync();
            if (user is null)
                return Error.Validation("Login.Failed", "البريد الإلكتروني أو ك غير صحيحة.");
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.User.PasswordHash);
            if (!isPasswordValid)
                return Error.Validation("Login.Failed", "أو كلمة المرور غير صحيحة.");

            string accessToken = _tokenService.GenerateAccessToken(user.User, user.TeacherId, user.StudentId);

            var refreshToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
                UserId = user.User.Id
            };

            var loginResponse = new LoginResponse(
                AccessToken: accessToken,
                RefreshToken: refreshToken.Token,
                Role: user.User.Role.ToString(),
                Name: user.User.Name
            );


            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);



            return loginResponse;
        }


    }
}
