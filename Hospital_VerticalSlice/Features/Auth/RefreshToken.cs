using FluentValidation;
using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Hospital_VerticalSlice.Features.Auth
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;

    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("لم يتم ارسال التوكن القديم");
        }
    }

    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public RefreshTokenHandler(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var existingToken = await _context.RefreshTokens
                .Where(x => x.Token == request.RefreshToken)
                .Select(x => new
                {
                    RefreshToken = x,
                    User = x.User,
                    TeacherId = x.User.Teacher != null ? (int?)x.User.Teacher.Id : null,
                    StudentId = x.User.Student != null ? (int?)x.User.Student.Id : null
                })
                .FirstOrDefaultAsync();

            if (existingToken is null || existingToken.RefreshToken.IsRevoked || existingToken.RefreshToken.ExpiresOnUtc <= DateTime.UtcNow)
            {
                return Error.Validation("Auth.InvalidToken", "رمز التحديث غير صالح أو منتهي الصلاحية.");
            }

            // 3. تعطيل التوكن القديم
            existingToken.RefreshToken.IsRevoked = true;
            // 4. توليد التوكنات الجديدة
            string newAccessToken = _tokenService.GenerateAccessToken(
                existingToken.User,
                existingToken.TeacherId,
                existingToken.StudentId);

            var userId = existingToken.User.Id;
            //var teacherId = existingToken.TeacherId;
            //var studentId = existingToken.StudentId;
            var role = existingToken.User.Role;
            var newRefreshToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
                UserId = userId
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new LoginResponse(
                AccessToken: newAccessToken,
                RefreshToken: newRefreshToken.Token,
                Role: role.ToString(),
                Name: existingToken.User.Name
            );
        }
    }
}

