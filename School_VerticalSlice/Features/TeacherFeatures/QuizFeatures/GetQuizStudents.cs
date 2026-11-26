using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;

namespace School_VerticalSlice.Features.TeacherFeatures.QuizFeatures
{
    public record QuizStudentItemResponse(
        int StudentId,
        string StudentName,
        float? TotalDegree
    );

    public record GetQuizStudentsQuery(int QuizId) : IRequest<Result<List<QuizStudentItemResponse>>>;

    public class GetQuizStudentsValidator : AbstractValidator<GetQuizStudentsQuery>
    {
        public GetQuizStudentsValidator()
        {
            RuleFor(x => x.QuizId).GreaterThan(0);
        }
    }

    public class GetQuizStudentsHandler : IRequestHandler<GetQuizStudentsQuery, Result<List<QuizStudentItemResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetQuizStudentsHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<List<QuizStudentItemResponse>>> Handle(GetQuizStudentsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId is null)
                return Error.Unauthorized("User is not authenticated.");

            var isQuizOwner = await _context.Quizzes
                .AsNoTracking()
                .AnyAsync(q => q.Id == request.QuizId && q.TeacherId == currentUserId && q.IsDeleted != true,
                    cancellationToken);

            if (!isQuizOwner)
                return Error.Unauthorized("You are not authorized to view this quiz or quiz was not found.");

            var students = await _context.StudentQuiz
                .AsNoTracking()
                .Where(sq => sq.QuizId == request.QuizId)
                .Select(sq => new QuizStudentItemResponse(
                    StudentId: sq.StudentId,
                    StudentName: sq.Student.User.Name,
                    TotalDegree: sq.TotalDegree
                ))
                .ToListAsync(cancellationToken);

            return students;
        }
    }
}