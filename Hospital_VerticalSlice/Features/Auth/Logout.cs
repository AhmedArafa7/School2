using School_VerticalSlice.Entities;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Auth
{
    public record LogoutCommand(string? RefreshToken) : IRequest<Result<string>>;

    public class LogoutHandler : IRequestHandler<LogoutCommand, Result<string>>
    {
        private readonly ApplicationDbContext _context;

        public LogoutHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(d=>d.Token == request.RefreshToken);

            if(token != null)
            {
                token.IsRevoked = true;

                await _context.SaveChangesAsync();
            }

            return "Loging out done";
        }
    }
}
