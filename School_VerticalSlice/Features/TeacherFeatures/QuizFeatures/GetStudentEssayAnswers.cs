using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;

namespace School_VerticalSlice.Features.TeacherFeatures.QuizFeatures
{
    public record GetStudentEssayAnswersResponse(
        int QuestionId,
        string Question,
        string? StudentAnswer,
        float MaxDegree
    );

    public record GetStudentEssayAnswersQuery(
        int QuizId,
        int StudentId
    ) : IRequest<Result<List<GetStudentEssayAnswersResponse>>>;

    public class GetStudentEssayAnswersValidator : AbstractValidator<GetStudentEssayAnswersQuery>
    {
        public GetStudentEssayAnswersValidator()
        {
            RuleFor(x => x.QuizId).GreaterThan(0);
            RuleFor(x => x.StudentId).GreaterThan(0);
        }
    }

    public class GetStudentEssayAnswersHandler : IRequestHandler<GetStudentEssayAnswersQuery, Result<List<GetStudentEssayAnswersResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetStudentEssayAnswersHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<List<GetStudentEssayAnswersResponse>>> Handle(GetStudentEssayAnswersQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId is null)
                return Error.Unauthorized("User is not authenticated.");

            var quizExistsForTeacher = await _context.Quizzes
                .AsNoTracking()
                .AnyAsync(q => q.Id == request.QuizId && q.TeacherId == currentUserId, cancellationToken);

            if (!quizExistsForTeacher)
                return Error.Unauthorized("You are not authorized to view this quiz or quiz was not found.");

            var hasStudentQuiz = await _context.StudentQuiz
                .AsNoTracking()
                .AnyAsync(sq => sq.QuizId == request.QuizId && sq.StudentId == request.StudentId, cancellationToken);

            if (!hasStudentQuiz)
                return Error.NotFound("StudentQuiz.NotFound", "This student hasn't entered this quiz.");


            var essayQuestions = await _context.Questions
                .AsNoTracking()
                .Where(q => q.QuizId == request.QuizId && (q.Choices == null || q.Choices.Count == 0))
                .Select(q => new
                {
                    QuestionId = q.Id,
                    QuestionText = q.question,
                    q.Degree
                })
                .ToListAsync(cancellationToken);

            if (essayQuestions.Count == 0)
                return Error.NotFound("EssayQuestions.NotFound", "This quiz does not contain any essay questions.");

            var essayQuestionIds = essayQuestions.Select(q => q.QuestionId).ToList();

            var studentAnswers = await _context.StudentAnswers
                .AsNoTracking()
                .Where(sa => sa.StudentId == request.StudentId && essayQuestionIds.Contains(sa.QuestionId))
                .Select(sa => new
                {
                    sa.QuestionId,
                    sa.StudentAnswer
                })
                .ToListAsync(cancellationToken);

            var answersByQuestionId = studentAnswers
                .GroupBy(sa => sa.QuestionId)
                .ToDictionary(g => g.Key, g => g.Last().StudentAnswer);

            var result = essayQuestions.Select(q =>
            {
                answersByQuestionId.TryGetValue(q.QuestionId, out var studentAns);

                return new GetStudentEssayAnswersResponse(
                    QuestionId: q.QuestionId,
                    Question: q.QuestionText,
                    StudentAnswer: studentAns,
                    MaxDegree: q.Degree
                );
            }).ToList();

            return result;
        }
    }
}