using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;

namespace School_VerticalSlice.Features.StudentFeatures.Quiz
{
    public record GetQuizReviewResponse(
        int QuestionId,
        string Question,
        List<string>? Choices,
        string? StudentAnswer,
        string? CorrectAnswer,
        float Degree
    );

    public record GetQuizReviewQuery(int QuizId) : IRequest<Result<List<GetQuizReviewResponse>>>;

    public class GetQuizReviewValidator : AbstractValidator<GetQuizReviewQuery>
    {
        public GetQuizReviewValidator()
        {
            RuleFor(x => x.QuizId).GreaterThan(0);
        }
    }

    public class GetQuizReviewHandler : IRequestHandler<GetQuizReviewQuery, Result<List<GetQuizReviewResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetQuizReviewHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<List<GetQuizReviewResponse>>> Handle(GetQuizReviewQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId is null)
                return Error.Unauthorized("User is not authenticated.");

            var isStudentInQuizClass = await _context.Quizzes
                .AsNoTracking()
                .AnyAsync(q => q.Id == request.QuizId
                            && q.Classes.Any(c => c.Students.Any(s => s.Id == currentUserId)),
                    cancellationToken);

            if (!isStudentInQuizClass)
                return Error.Unauthorized("You are not authorized to view this quiz review or quiz was not found.");

            var hasStudentQuiz = await _context.StudentQuiz
                .AsNoTracking()
                .AnyAsync(sq => sq.QuizId == request.QuizId && sq.StudentId == currentUserId,
                    cancellationToken);

            if (!hasStudentQuiz)
                return Error.NotFound("StudentQuiz.NotFound", "You haven't entered or submitted this quiz yet.");

            var questions = await _context.Questions
                .AsNoTracking()
                .Where(q => q.QuizId == request.QuizId)
                .Select(q => new
                {
                    QuestionId = q.Id,
                    QuestionText = q.question,
                    q.Choices,
                    q.CorrectAnswer,
                    q.Degree
                })
                .ToListAsync(cancellationToken);

            if (questions.Count == 0)
                return Error.NotFound("Questions.NotFound", "No questions found for this quiz.");

            var questionIds = questions.Select(q => q.QuestionId).ToList();

            var studentAnswers = await _context.StudentAnswers
                .AsNoTracking()
                .Where(sa => sa.StudentId == currentUserId && questionIds.Contains(sa.QuestionId))
                .Select(sa => new
                {
                    sa.QuestionId,
                    sa.StudentAnswer
                })
                .ToListAsync(cancellationToken);

            var answersByQuestionId = studentAnswers
                .GroupBy(sa => sa.QuestionId)
                .ToDictionary(g => g.Key, g => g.Last().StudentAnswer);

            var result = questions.Select(q =>
            {
                answersByQuestionId.TryGetValue(q.QuestionId, out var studentAns);

                return new GetQuizReviewResponse(
                    QuestionId: q.QuestionId,
                    Question: q.QuestionText,
                    Choices: q.Choices,
                    StudentAnswer: studentAns,
                    CorrectAnswer: q.CorrectAnswer,
                    Degree: q.Degree
                );
            }).ToList();

            return result;
        }
    }
}