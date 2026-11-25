using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Features.Admin;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.StudentFeatures
{

    public record GetQuizDegreeCommand(int QuizId) : IRequest<Result<float>>;

    public class GetQuizDegreeHandler : IRequestHandler<GetQuizDegreeCommand, Result<float>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetQuizDegreeHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<float>> Handle(GetQuizDegreeCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var isAutherized = await _context.Quizzes
                .AnyAsync(q => q.Classes.Any(c => c.Students.Any(s => s.Id == currentUserId))
                         && q.IsDeleted == true);

            if (isAutherized)
                return Error.Unauthorized("You are unauthorized to that Quiz");

            var quizDegree = await _context.StudentQuiz
                .Select(sq => sq.TotalDegree)
                .FirstOrDefaultAsync();

            if (quizDegree == null)
                return Error.NotFound("You didn't enter that Quiz");


            return quizDegree;
        }
    }
}

